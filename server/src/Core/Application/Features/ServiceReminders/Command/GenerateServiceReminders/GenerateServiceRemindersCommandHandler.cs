using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Features.ServiceReminders.Query;

using Domain.Entities;
using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.ServiceReminders.Command.GenerateServiceReminders;

/// <summary>
/// Generates calculated reminders from active schedules and persists them.
/// Produces many OVERDUE/DUE_SOON reminders, but at most one UPCOMING per (VehicleID, ServiceScheduleID).
/// </summary>
public class GenerateServiceRemindersCommandHandler : IRequestHandler<GenerateServiceRemindersCommand, GenerateServiceRemindersResponse>
{
    // Constants
    private const int MaxOccurrenceCount = 100;
    private const int DaysPerWeek = 7;

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

            // Fetch all active schedules with required vehicle/task data in one go (avoid N+1)
            var schedules = await _serviceReminderRepository.GetActiveServiceSchedulesWithDataAsync();

            // Ignore soft-deleted schedules if any slipped through
            schedules = schedules.Where(s => !s.IsSoftDeleted && s.IsActive).ToList();

            // Calculate reminders in-memory for current time; generation enforces a single UPCOMING per pair
            var calculated = GenerateCalculatedReminders(schedules, DateTime.UtcNow);

            // Persist in bulk; repository preserves COMPLETED and updates existing non-final reminders
            await _serviceReminderRepository.SyncRemindersAsync(calculated);

            _logger.LogInformation("Successfully generated {Count} service reminders", calculated.Count);

            return new GenerateServiceRemindersResponse(
                GeneratedCount: calculated.Count,
                UpdatedCount: calculated.Count,
                Success: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate service reminders");
            return new GenerateServiceRemindersResponse(0, 0, false, ex.Message);
        }
    }

    /// <summary>
    /// For each active schedule and assigned vehicle, yields OVERDUE/DUE_SOON occurrences
    /// and exactly one UPCOMING occurrence (the next upcoming)
    /// </summary>
    private static List<ServiceReminderDTO> GenerateCalculatedReminders(List<ServiceSchedule> schedules, DateTime currentDateUtc)
    {
        var reminders = new List<ServiceReminderDTO>();

        foreach (var schedule in schedules)
        {
            var vehicleAssignments = schedule.ServiceProgram?.XrefServiceProgramVehicles ?? [];
            var tasks = schedule.XrefServiceScheduleServiceTasks?.Select(x => x.ServiceTask).ToList() ?? [];
            if (tasks.Count == 0) continue;

            if (!schedule.HasExactlyOneScheduleType())
            {
                throw new InvalidOperationException($"ServiceSchedule {schedule.ID} violates XOR constraint - must be either time-based OR mileage-based");
            }

            foreach (var assignment in vehicleAssignments)
            {
                if (assignment.Vehicle is null) continue;

                reminders.AddRange(GenerateForVehicleSchedule(schedule, assignment.Vehicle, tasks, assignment.AddedAt, currentDateUtc));
            }
        }

        return reminders;
    }

    private static IEnumerable<ServiceReminderDTO> GenerateForVehicleSchedule(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> tasks,
        DateTime assignmentDateUtc,
        DateTime currentDateUtc)
    {
        // 1) Overdue & Due-Soon (may be multiple)
        foreach (var dto in GenerateNonUpcoming(schedule, vehicle, tasks, assignmentDateUtc, currentDateUtc))
        {
            yield return dto;
        }

        // 2) Exactly ONE Upcoming (the next upcoming occurrence only)
        var upcoming = GenerateSingleUpcoming(schedule, vehicle, tasks, assignmentDateUtc, currentDateUtc);
        if (upcoming != null) yield return upcoming;
    }

    private static IEnumerable<ServiceReminderDTO> GenerateNonUpcoming(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> tasks,
        DateTime assignmentDateUtc,
        DateTime currentDateUtc)
    {
        if (schedule.IsTimeBased())
        {
            return GenerateTimeBasedOccurrences(schedule, vehicle, tasks, assignmentDateUtc, currentDateUtc, includeOnlyUpcoming: false);
        }

        return GenerateMileageBasedOccurrences(schedule, vehicle, tasks, assignmentDateUtc, currentDateUtc, includeOnlyUpcoming: false);
    }

    private static ServiceReminderDTO? GenerateSingleUpcoming(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> tasks,
        DateTime assignmentDateUtc,
        DateTime currentDateUtc)
    {
        if (schedule.IsTimeBased())
        {
            return GenerateTimeBasedOccurrences(schedule, vehicle, tasks, assignmentDateUtc, currentDateUtc, includeOnlyUpcoming: true)
                .FirstOrDefault();
        }

        return GenerateMileageBasedOccurrences(schedule, vehicle, tasks, assignmentDateUtc, currentDateUtc, includeOnlyUpcoming: true)
            .FirstOrDefault();
    }

    private static IEnumerable<ServiceReminderDTO> GenerateTimeBasedOccurrences(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> tasks,
        DateTime assignmentDateUtc,
        DateTime currentDateUtc,
        bool includeOnlyUpcoming)
    {
        // includeOnlyUpcoming: when true, yield only the first UPCOMING and stop; otherwise yield OVERDUE/DUE_SOON
        var startDate = schedule.FirstServiceDate ?? assignmentDateUtc;

        for (int occurrence = 1; occurrence <= MaxOccurrenceCount; occurrence++)
        {
            var dueDate = occurrence == 1
                ? startDate
                : CalculateOccurrenceDate(startDate, schedule.TimeIntervalValue!.Value, schedule.TimeIntervalUnit!.Value, occurrence - 1);

            var status = schedule.CalculateReminderStatus(dueDate, null, currentDateUtc, vehicle.Mileage);

            if (includeOnlyUpcoming)
            {
                if (status == ServiceReminderStatusEnum.UPCOMING)
                {
                    yield return CreateReminder(schedule, vehicle, tasks, dueDate, null, status, occurrence, ServiceScheduleTypeEnum.TIME, currentDateUtc);
                    yield break; // only the next upcoming
                }

                // Sanity horizon: avoid infinite search if configuration pushes occurrences far ahead
                if (dueDate > currentDateUtc.AddYears(10)) yield break;
            }
            else
            {
                if (status == ServiceReminderStatusEnum.OVERDUE || status == ServiceReminderStatusEnum.DUE_SOON)
                {
                    yield return CreateReminder(schedule, vehicle, tasks, dueDate, null, status, occurrence, ServiceScheduleTypeEnum.TIME, currentDateUtc);
                }
            }
        }
    }

    private static IEnumerable<ServiceReminderDTO> GenerateMileageBasedOccurrences(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> tasks,
        DateTime assignmentDateUtc,
        DateTime currentDateUtc,
        bool includeOnlyUpcoming)
    {
        // includeOnlyUpcoming: when true, yield only the first UPCOMING and stop; otherwise yield OVERDUE/DUE_SOON
        var startMileage = schedule.FirstServiceMileage ?? vehicle.Mileage;

        for (int occurrence = 1; occurrence <= MaxOccurrenceCount; occurrence++)
        {
            var dueMileage = startMileage + (schedule.MileageInterval!.Value * (occurrence - 1));

            var status = schedule.CalculateReminderStatus(null, dueMileage, currentDateUtc, vehicle.Mileage);

            if (includeOnlyUpcoming)
            {
                if (status == ServiceReminderStatusEnum.UPCOMING)
                {
                    yield return CreateReminder(schedule, vehicle, tasks, null, dueMileage, status, occurrence, ServiceScheduleTypeEnum.MILEAGE, currentDateUtc);
                    yield break; // only the next upcoming
                }

                // Sanity horizon: avoid infinite search if vehicle is far behind the next occurrences
                if (dueMileage > vehicle.Mileage + (schedule.MileageInterval.Value * 100)) yield break;
            }
            else
            {
                if (status == ServiceReminderStatusEnum.OVERDUE || status == ServiceReminderStatusEnum.DUE_SOON)
                {
                    yield return CreateReminder(schedule, vehicle, tasks, null, dueMileage, status, occurrence, ServiceScheduleTypeEnum.MILEAGE, currentDateUtc);
                }
            }
        }
    }

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
    /// Construct a DTO snapshot for the computed occurrence with derived aggregates (cost/hours/tasks).
    /// </summary>
    private static ServiceReminderDTO CreateReminder(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> tasks,
        DateTime? dueDate,
        double? dueMileage,
        ServiceReminderStatusEnum status,
        int occurrenceNumber,
        ServiceScheduleTypeEnum scheduleType,
        DateTime currentDateUtc)
    {
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

        var priorityLevel = status switch
        {
            ServiceReminderStatusEnum.OVERDUE => PriorityLevelEnum.HIGH,
            ServiceReminderStatusEnum.DUE_SOON => PriorityLevelEnum.MEDIUM,
            _ => PriorityLevelEnum.LOW
        };

        var mileageVariance = dueMileage.HasValue ? vehicle.Mileage - dueMileage.Value : (double?)null;
        var daysUntilDue = dueDate.HasValue ? (int)(dueDate.Value - currentDateUtc).TotalDays : (int?)null;

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