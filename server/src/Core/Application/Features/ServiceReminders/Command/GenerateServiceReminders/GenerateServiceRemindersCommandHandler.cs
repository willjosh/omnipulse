using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Features.ServiceReminders.Query;

using Domain.Entities;
using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.ServiceReminders.Command.GenerateServiceReminders;

public class GenerateServiceRemindersCommandHandler : IRequestHandler<GenerateServiceRemindersCommand, GenerateServiceRemindersResponse>
{
    // Constants
    private const int MaxOccurrenceCount = 100;
    private const int DaysPerWeek = 7;
    private const int MaxUpcomingLookaheadYears = 1;
    private const int MaxUpcomingLookaheadKm = 10000; // 10,000 km lookahead (equivalent to ~1 year at 30 km/day)

    private readonly IServiceReminderRepository _serviceReminderRepository;
    private readonly IAppLogger<GenerateServiceRemindersCommandHandler> _logger;

    public GenerateServiceRemindersCommandHandler(
        IServiceReminderRepository serviceReminderRepository,
        IAppLogger<GenerateServiceRemindersCommandHandler> logger)
    {
        _serviceReminderRepository = serviceReminderRepository;
        _logger = logger;
    }

    public async Task<GenerateServiceRemindersResponse> Handle(GenerateServiceRemindersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting service reminder generation");

            // GENERATE: Get raw data and calculate reminders using domain logic
            var serviceSchedulesWithData = await _serviceReminderRepository.GetActiveServiceSchedulesWithDataAsync();
            var calculatedReminders = await GenerateCalculatedServiceRemindersAsync(serviceSchedulesWithData);

            // SYNC: Persist calculated reminders to database
            await _serviceReminderRepository.SyncRemindersAsync(calculatedReminders);

            _logger.LogInformation("Successfully generated {Count} service reminders", calculatedReminders.Count);

            return new GenerateServiceRemindersResponse(
                GeneratedCount: calculatedReminders.Count,
                UpdatedCount: calculatedReminders.Count,
                Success: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate service reminders");
            return new GenerateServiceRemindersResponse(
                GeneratedCount: 0,
                UpdatedCount: 0,
                Success: false,
                ErrorMessage: ex.Message);
        }
    }

    /// <summary>Generate calculated service reminders from service schedules</summary>
    private static Task<List<ServiceReminderDTO>> GenerateCalculatedServiceRemindersAsync(List<ServiceSchedule> serviceSchedules)
    {
        var currentDate = DateTime.UtcNow;
        var reminders = new List<ServiceReminderDTO>();

        foreach (var schedule in serviceSchedules)
        {
            var vehicleAssignments = schedule.ServiceProgram?.XrefServiceProgramVehicles ?? [];
            var serviceTasks = schedule.XrefServiceScheduleServiceTasks?.Select(x => x.ServiceTask).ToList() ?? [];

            foreach (var vehicleAssignment in vehicleAssignments)
            {
                var vehicle = vehicleAssignment.Vehicle;
                if (vehicle == null) continue;

                var scheduleReminders = GenerateRemindersForSchedule(
                    schedule,
                    vehicle,
                    serviceTasks,
                    vehicleAssignment.AddedAt,
                    currentDate);

                reminders.AddRange(scheduleReminders);
            }
        }

        return Task.FromResult(reminders);
    }

    private static List<ServiceReminderDTO> GenerateRemindersForSchedule(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> serviceTasks,
        DateTime assignmentDate,
        DateTime currentDate)
    {
        var reminders = new List<ServiceReminderDTO>();

        if (serviceTasks.Count == 0)
        {
            return reminders;
        }

        // Validate XOR constraint - ServiceSchedule must be either time-based OR mileage-based
        if (!schedule.HasExactlyOneScheduleType())
        {
            throw new InvalidOperationException($"ServiceSchedule {schedule.ID} violates XOR constraint - must be either time-based OR mileage-based, not both or neither");
        }

        // Generate overdue and due soon reminders (these can be multiple)
        var overdueAndDueSoonReminders = GenerateOverdueAndDueSoonReminders(
            schedule, vehicle, serviceTasks, assignmentDate, currentDate);
        reminders.AddRange(overdueAndDueSoonReminders);

        // Generate the next upcoming reminder (only one per schedule)
        var nextUpcomingReminder = GenerateNextUpcomingReminder(
            schedule, vehicle, serviceTasks, assignmentDate, currentDate);
        if (nextUpcomingReminder != null)
        {
            reminders.Add(nextUpcomingReminder);
        }

        return reminders;
    }

    /// <summary>Generate overdue and due soon reminders respecting XOR constraint</summary>
    private static List<ServiceReminderDTO> GenerateOverdueAndDueSoonReminders(ServiceSchedule schedule, Vehicle vehicle, List<ServiceTask> serviceTasks, DateTime assignmentDate, DateTime currentDate)
    {
        var reminders = new List<ServiceReminderDTO>();

        // XOR constraint: Generate reminders based on exactly one schedule type
        if (schedule.IsTimeBased())
        {
            var timeBasedReminders = GenerateTimeBasedReminders(schedule, vehicle, serviceTasks, assignmentDate, currentDate, excludeUpcoming: true);
            reminders.AddRange(timeBasedReminders);
        }
        else if (schedule.IsMileageBased())
        {
            var mileageBasedReminders = GenerateMileageBasedReminders(schedule, vehicle, serviceTasks, assignmentDate, currentDate, excludeUpcoming: true);
            reminders.AddRange(mileageBasedReminders);
        }

        return reminders;
    }

    /// <summary>Generate next upcoming reminder respecting XOR constraint</summary>
    private static ServiceReminderDTO? GenerateNextUpcomingReminder(ServiceSchedule schedule, Vehicle vehicle, List<ServiceTask> serviceTasks, DateTime assignmentDate, DateTime currentDate)
    {
        var upcomingReminders = new List<ServiceReminderDTO>();

        // XOR constraint: Generate upcoming reminder based on exactly one schedule type
        if (schedule.IsTimeBased())
        {
            var timeBasedReminders = GenerateTimeBasedReminders(schedule, vehicle, serviceTasks, assignmentDate, currentDate, excludeUpcoming: false);
            upcomingReminders.AddRange(timeBasedReminders.Where(r => r.Status == ServiceReminderStatusEnum.UPCOMING));
        }
        else if (schedule.IsMileageBased())
        {
            var mileageBasedReminders = GenerateMileageBasedReminders(schedule, vehicle, serviceTasks, assignmentDate, currentDate, excludeUpcoming: false);
            upcomingReminders.AddRange(mileageBasedReminders.Where(r => r.Status == ServiceReminderStatusEnum.UPCOMING));
        }

        // Return the first upcoming reminder (there should only be one type due to XOR)
        return upcomingReminders.FirstOrDefault();
    }

    /// <summary>
    /// Generates reminders for time-based schedules
    /// </summary>
    private static List<ServiceReminderDTO> GenerateTimeBasedReminders(ServiceSchedule schedule, Vehicle vehicle, List<ServiceTask> serviceTasks, DateTime assignmentDate, DateTime currentDate, bool excludeUpcoming)
    {
        var reminders = new List<ServiceReminderDTO>();
        var startDate = schedule.FirstServiceDate ?? assignmentDate;

        // Generate occurrences and determine their status using domain extensions
        for (int occurrenceNumber = 1; occurrenceNumber <= MaxOccurrenceCount; occurrenceNumber++)
        {
            // Calculate due date: first occurrence is the start date, subsequent ones add intervals
            var dueDate = occurrenceNumber == 1
                ? startDate
                : CalculateOccurrenceDate(startDate, schedule.TimeIntervalValue!.Value, schedule.TimeIntervalUnit!.Value, occurrenceNumber - 1);

            // Stop if we're too far in the future
            if (dueDate > currentDate.AddYears(MaxUpcomingLookaheadYears)) break;

            // Calculate status using domain logic
            var status = schedule.CalculateReminderStatus(dueDate, null, currentDate, vehicle.Mileage);

            // Filter based on excludeUpcoming flag
            if (excludeUpcoming && status == ServiceReminderStatusEnum.UPCOMING) continue;
            if (!excludeUpcoming && status != ServiceReminderStatusEnum.UPCOMING) continue;

            // Create and add the reminder DTO
            var reminderDto = CreateServiceReminderDTO(schedule, vehicle, serviceTasks, dueDate, null, status, occurrenceNumber, ServiceScheduleTypeEnum.TIME);
            reminders.Add(reminderDto);
        }

        return reminders;
    }

    /// <summary>
    /// Generates reminders for mileage-based schedules
    /// </summary>
    private static List<ServiceReminderDTO> GenerateMileageBasedReminders(ServiceSchedule schedule, Vehicle vehicle, List<ServiceTask> serviceTasks, DateTime assignmentDate, DateTime currentDate, bool excludeUpcoming)
    {
        var reminders = new List<ServiceReminderDTO>();

        // Determine the starting mileage for generating occurrences
        var startMileage = schedule.FirstServiceMileage ?? vehicle.Mileage;

        // Calculate how many intervals have passed since the first service
        var intervalsPassed = 0;
        if (schedule.FirstServiceMileage.HasValue && vehicle.Mileage > schedule.FirstServiceMileage.Value)
        {
            intervalsPassed = (int)Math.Floor((vehicle.Mileage - schedule.FirstServiceMileage.Value) / schedule.MileageInterval!.Value);
        }

        // Generate occurrences starting from the first service mileage
        for (int occurrenceNumber = 1; occurrenceNumber <= MaxOccurrenceCount; occurrenceNumber++)
        {
            var dueMileage = startMileage + (schedule.MileageInterval!.Value * (occurrenceNumber - 1));

            // Stop if we're too far in the future (look ahead reasonable distance)
            var maxLookAheadMileage = vehicle.Mileage + MaxUpcomingLookaheadKm;
            if (dueMileage > maxLookAheadMileage) break;

            // Calculate status using domain logic
            var status = schedule.CalculateReminderStatus(null, dueMileage, currentDate, vehicle.Mileage);

            // Filter based on excludeUpcoming flag
            if (excludeUpcoming && status == ServiceReminderStatusEnum.UPCOMING) continue;
            if (!excludeUpcoming && status != ServiceReminderStatusEnum.UPCOMING) continue;

            // Create and add the reminder DTO
            var reminderDto = CreateServiceReminderDTO(schedule, vehicle, serviceTasks, null, dueMileage, status, occurrenceNumber, ServiceScheduleTypeEnum.MILEAGE);
            reminders.Add(reminderDto);
        }

        return reminders;
    }

    /// <summary>
    /// Helper: Calculate occurrence date for time-based schedules
    /// </summary>
    private static DateTime CalculateOccurrenceDate(DateTime startDate, int intervalValue, TimeUnitEnum intervalUnit, int occurrenceNumber)
    {
        var totalIntervalValue = intervalValue * occurrenceNumber;

        return intervalUnit switch
        {
            TimeUnitEnum.Hours => startDate.AddHours(totalIntervalValue),
            TimeUnitEnum.Days => startDate.AddDays(totalIntervalValue),
            TimeUnitEnum.Weeks => startDate.AddDays(totalIntervalValue * DaysPerWeek),
            _ => throw new ArgumentException($"Unsupported time unit: {intervalUnit}")
        };
    }

    /// <summary>
    /// Helper: Create ServiceReminderDTO with all required properties
    /// </summary>
    private static ServiceReminderDTO CreateServiceReminderDTO(ServiceSchedule schedule, Vehicle vehicle, List<ServiceTask> tasks, DateTime? dueDate, double? dueMileage, ServiceReminderStatusEnum status, int occurrenceNumber, ServiceScheduleTypeEnum scheduleType)
    {
        var currentDate = DateTime.UtcNow;

        // Create ServiceTaskInfo DTOs
        var serviceTasks = tasks.Select(task => new ServiceTaskInfoDTO
        {
            ServiceTaskID = task.ID,
            ServiceTaskName = task.Name,
            ServiceTaskCategory = task.Category,
            EstimatedLabourHours = task.EstimatedLabourHours,
            EstimatedCost = task.EstimatedCost,
            Description = task.Description,
            IsRequired = true
        }).ToList();

        // Calculate priority level and other computed values directly
        var priorityLevel = status switch
        {
            ServiceReminderStatusEnum.OVERDUE => PriorityLevelEnum.HIGH,
            ServiceReminderStatusEnum.DUE_SOON => PriorityLevelEnum.MEDIUM,
            ServiceReminderStatusEnum.UPCOMING => PriorityLevelEnum.LOW,
            _ => PriorityLevelEnum.LOW
        };
        var mileageVariance = dueMileage.HasValue ? vehicle.Mileage - dueMileage.Value : (double?)null;
        var daysUntilDue = dueDate.HasValue ? (int)(dueDate.Value - currentDate).TotalDays : (int?)null;

        return new ServiceReminderDTO
        {
            ID = 0, // Placeholder ID for calculated reminders (will be set when persisted)
            WorkOrderID = null, // Will be set when AddServiceReminderToExistingWorkOrderCommand is called
            VehicleID = vehicle.ID,
            VehicleName = vehicle.Name,
            ServiceProgramID = schedule.ServiceProgramID,
            ServiceProgramName = schedule.ServiceProgram?.Name,
            ServiceScheduleID = schedule.ID,
            ServiceScheduleName = schedule.Name,
            ServiceTasks = serviceTasks,
            TotalEstimatedLabourHours = serviceTasks.Sum(t => t.EstimatedLabourHours),
            TotalEstimatedCost = serviceTasks.Sum(t => t.EstimatedCost),
            TaskCount = serviceTasks.Count,
            DueDate = dueDate,
            DueMileage = dueMileage,
            Status = status,
            PriorityLevel = priorityLevel,
            TimeIntervalValue = scheduleType == ServiceScheduleTypeEnum.TIME ? schedule.TimeIntervalValue : null,
            TimeIntervalUnit = scheduleType == ServiceScheduleTypeEnum.TIME ? schedule.TimeIntervalUnit : null,
            MileageInterval = scheduleType == ServiceScheduleTypeEnum.MILEAGE ? schedule.MileageInterval : null,
            TimeBufferValue = scheduleType == ServiceScheduleTypeEnum.TIME ? schedule.TimeBufferValue : null,
            TimeBufferUnit = scheduleType == ServiceScheduleTypeEnum.TIME ? schedule.TimeBufferUnit : null,
            MileageBuffer = scheduleType == ServiceScheduleTypeEnum.MILEAGE ? schedule.MileageBuffer : null,
            CurrentMileage = vehicle.Mileage,
            MileageVariance = mileageVariance,
            DaysUntilDue = daysUntilDue,
            OccurrenceNumber = occurrenceNumber,
            ScheduleType = scheduleType
        };
    }
}