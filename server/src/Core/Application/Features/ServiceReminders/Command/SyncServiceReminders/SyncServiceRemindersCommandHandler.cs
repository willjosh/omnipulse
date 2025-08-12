using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Contracts.Services;
using Application.Features.ServiceReminders.Query;

using Domain.Entities;
using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.ServiceReminders.Command.SyncServiceReminders;

/// <summary>
/// Syncs reminders from active schedules: calculates occurrences and persists them.
/// Produces many OVERDUE/DUE_SOON reminders, but at most one UPCOMING per (VehicleID, ServiceScheduleID).
/// </summary>
public class SyncServiceRemindersCommandHandler : IRequestHandler<SyncServiceRemindersCommand, SyncServiceRemindersResponse>
{
    // Constants
    private const int DaysPerWeek = 7;

    private readonly IServiceReminderRepository _serviceReminderRepository;
    private readonly IAppLogger<SyncServiceRemindersCommandHandler> _logger;
    private readonly IServiceReminderStatusUpdater _serviceReminderStatusUpdater;
    private readonly TimeProvider _timeProvider;

    public SyncServiceRemindersCommandHandler(
        IServiceReminderRepository serviceReminderRepository,
        IAppLogger<SyncServiceRemindersCommandHandler> logger,
        TimeProvider timeProvider,
        IServiceReminderStatusUpdater serviceReminderStatusUpdater)
    {
        _serviceReminderRepository = serviceReminderRepository;
        _logger = logger;
        _timeProvider = timeProvider;
        _serviceReminderStatusUpdater = serviceReminderStatusUpdater;
    }

    public async Task<SyncServiceRemindersResponse> Handle(SyncServiceRemindersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting service reminder sync");

            // Cleanup: remove unlinked/orphaned reminders
            var deletedOrphans = await _serviceReminderRepository.DeleteAllUnlinkedReminders(cancellationToken);
            if (deletedOrphans > 0)
            {
                _logger.LogInformation("Cleanup removed {Count} unlinked service reminders", deletedOrphans);
            }

            // Update statuses first so existing UPCOMINGs can shift to DUE_SOON/OVERDUE before insert
            await _serviceReminderStatusUpdater.UpdateAllReminderStatusesAsync(cancellationToken);

            // Fetch all active schedules with required vehicle/task data in one go (avoid N+1)
            var schedules = await _serviceReminderRepository.GetActiveServiceSchedulesWithDataAsync(cancellationToken);

            // Ignore soft-deleted schedules if any slipped through
            schedules = schedules.Where(s => !s.IsSoftDeleted).ToList();

            // Calculate reminders in-memory for current time; generation enforces a single UPCOMING per pair
            var calculated = GenerateCalculatedReminders(schedules, _timeProvider.GetUtcNow().UtcDateTime);

            // Persist only new reminders; existing ones are skipped (idempotent)
            var inserted = await _serviceReminderRepository.AddNewRemindersAsync(calculated, cancellationToken);

            _logger.LogInformation("Synced service reminders: inserted {Inserted} (cleanup removed {DeletedOrphans})", inserted, deletedOrphans);

            return new SyncServiceRemindersResponse(
                GeneratedCount: inserted,
                Success: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync service reminders");
            return new SyncServiceRemindersResponse(0, false, ex.Message);
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

                reminders.AddRange(GenerateForVehicleAndSchedulePair(
                    schedule,
                    assignment.Vehicle,
                    tasks,
                    assignment.AddedAt,
                    currentDateUtc,
                    assignment.VehicleMileageAtAssignment));
            }
        }

        return reminders;
    }

    /// <summary>
    /// Generates all reminder occurrences for a given schedule-vehicle pair:
    /// overdue and due-soon occurrences, plus exactly one upcoming occurrence.
    /// </summary>
    /// <param name="schedule">The source service schedule.</param>
    /// <param name="vehicle">The vehicle assigned to the schedule's program.</param>
    /// <param name="tasks">The service tasks belonging to the schedule.</param>
    /// <param name="assignmentDateUtc">The date the vehicle was assigned to the program (UTC).</param>
    /// <param name="currentDateUtc">Current UTC date for calculations.</param>
    /// <param name="vehicleMileageAtAssignment"></param>
    private static IEnumerable<ServiceReminderDTO> GenerateForVehicleAndSchedulePair(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> tasks,
        DateTime assignmentDateUtc,
        DateTime currentDateUtc,
        double? vehicleMileageAtAssignment)
    {
        if (schedule.IsTimeBased())
        {
            foreach (var dto in GenerateTimeBasedOccurrences(schedule, vehicle, tasks, assignmentDateUtc, currentDateUtc))
            {
                yield return dto;
            }
        }
        else
        {
            foreach (var dto in GenerateMileageBasedOccurrences(schedule, vehicle, tasks, currentDateUtc, vehicleMileageAtAssignment))
            {
                yield return dto;
            }
        }
    }

    /// <summary>
    /// Generates time-based occurrences for a schedule: yields all OVERDUE/DUE_SOON occurrences
    /// followed by exactly one UPCOMING occurrence (the next upcoming).
    /// </summary>
    /// <param name="schedule">The time-based schedule.</param>
    /// <param name="vehicle">The vehicle.</param>
    /// <param name="tasks">The tasks belonging to the schedule.</param>
    /// <param name="assignmentDateUtc">The assignment date (UTC).</param>
    /// <param name="currentDateUtc">Current UTC date.</param>
    private static IEnumerable<ServiceReminderDTO> GenerateTimeBasedOccurrences(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> tasks,
        DateTime assignmentDateUtc,
        DateTime currentDateUtc)
    {
        var startDate = schedule.FirstServiceDate ?? CalculateOccurrenceDate(assignmentDateUtc, schedule.TimeIntervalValue!.Value, schedule.TimeIntervalUnit!.Value, 1);

        // Precompute all due dates up to and including the first UPCOMING
        var timeDuePoints = GetTimeDuePoints(startDate,
            schedule.TimeIntervalValue!.Value,
            schedule.TimeIntervalUnit!.Value,
            currentDateUtc,
            schedule.TimeBufferValue ?? 0,
            schedule.TimeBufferUnit ?? TimeUnitEnum.Days);

        // Emit overdue/due-soon occurrences (all but last), then exactly one UPCOMING (last)
        for (int i = 0; i < timeDuePoints.Count; i++)
        {
            var isLast = i == timeDuePoints.Count - 1;
            var dueDate = timeDuePoints[i];
            var status = schedule.CalculateTimeReminderStatus(dueDate, currentDateUtc);
            if (!isLast)
            {
                if (status == ServiceReminderStatusEnum.OVERDUE || status == ServiceReminderStatusEnum.DUE_SOON)
                {
                    yield return CreateReminder(schedule, vehicle, tasks, dueDate, null, status, ServiceScheduleTypeEnum.TIME, currentDateUtc);
                }
            }
            else
            {
                if (status == ServiceReminderStatusEnum.UPCOMING)
                {
                    yield return CreateReminder(schedule, vehicle, tasks, dueDate, null, status, ServiceScheduleTypeEnum.TIME, currentDateUtc);
                }
            }
        }
    }

    /// <summary>
    /// Generates mileage-based occurrences for a schedule: yields all OVERDUE/DUE_SOON occurrences
    /// followed by exactly one UPCOMING occurrence (the next upcoming), using the schedule's mileage buffer
    /// as the due-soon threshold.
    /// </summary>
    /// <param name="schedule">The mileage-based schedule.</param>
    /// <param name="vehicle">The vehicle.</param>
    /// <param name="tasks">The tasks belonging to the schedule.</param>
    /// <param name="currentDateUtc">Current UTC date.</param>
    /// <param name="vehicleMileageAtAssignment"></param>
    private static IEnumerable<ServiceReminderDTO> GenerateMileageBasedOccurrences(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> tasks,
        DateTime currentDateUtc,
        double? vehicleMileageAtAssignment)
    {
        var interval = schedule.MileageInterval!.Value;
        var startMileage = schedule.FirstServiceMileage ?? ((vehicleMileageAtAssignment ?? vehicle.Mileage) + interval);

        // Get all due points up to and including the first one after current mileage
        var dueSoonThreshold = schedule.MileageBuffer ?? 0d;
        var duePoints = GetMileageDuePoints(startMileage, interval, dueSoonThreshold, vehicle.Mileage);

        if (duePoints.Count == 0) yield break;

        // Emit overdue/due-soon occurrences (all but last), then exactly one UPCOMING (last)
        for (int i = 0; i < duePoints.Count; i++)
        {
            var isLast = i == duePoints.Count - 1;
            var dueMileage = duePoints[i];
            var status = schedule.CalculateMileageReminderStatus(dueMileage, vehicle.Mileage);
            if (!isLast)
            {
                if (status == ServiceReminderStatusEnum.OVERDUE || status == ServiceReminderStatusEnum.DUE_SOON)
                {
                    yield return CreateReminder(schedule, vehicle, tasks, null, dueMileage, status, ServiceScheduleTypeEnum.MILEAGE, currentDateUtc);
                }
            }
            else
            {
                if (status == ServiceReminderStatusEnum.UPCOMING)
                {
                    yield return CreateReminder(schedule, vehicle, tasks, null, dueMileage, status, ServiceScheduleTypeEnum.MILEAGE, currentDateUtc);
                }
            }
        }
    }

    /// <summary>
    /// Calculates the due date for an occurrence number given a start date, interval value and unit.
    /// </summary>
    /// <param name="startDate">The base date (UTC).</param>
    /// <param name="intervalValue">Interval magnitude.</param>
    /// <param name="intervalUnit">Interval unit (Hours/Days/Weeks).</param>
    /// <param name="occurrenceNumber">1-based occurrence number.</param>
    /// <returns>The calculated due date.</returns>
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
    /// Returns all time due points starting at <paramref name="timeStartUtc"/> with interval (<paramref name="timeIntervalValue"/>, <paramref name="timeIntervalUnit"/>),
    /// stopping after including the first due date that would be <see cref="ServiceReminderStatusEnum.UPCOMING"/> given <paramref name="nowUtc"/> and the due-soon threshold
    /// (<paramref name="dueSoonValue"/>, <paramref name="dueSoonUnit"/>).
    /// </summary>
    private static List<DateTime> GetTimeDuePoints(
        DateTime timeStartUtc,
        int timeIntervalValue,
        TimeUnitEnum timeIntervalUnit,
        DateTime nowUtc,
        int dueSoonValue,
        TimeUnitEnum dueSoonUnit)
    {
        var duePoints = new List<DateTime>();
        DateTime duePoint = timeStartUtc;
        while (true)
        {
            duePoints.Add(duePoint);
            if (ServiceReminderExtensions.IsUpcomingByTime(nowUtc, duePoint, dueSoonValue, dueSoonUnit)) break; // included the first UPCOMING due point
            duePoint = CalculateOccurrenceDate(duePoint, timeIntervalValue, timeIntervalUnit, 1);
        }
        return duePoints;
    }

    /// <summary>
    /// Returns all mileage due points starting at <paramref name="mileageStart"/> with step <paramref name="mileageInterval"/>,
    /// stopping after including the first due point that would be <see cref="ServiceReminderStatusEnum.UPCOMING"/>.
    /// A due point is <see cref="ServiceReminderStatusEnum.UPCOMING"/> when
    /// <paramref name="mileageCurrent"/> &lt; (dueMileage - <paramref name="mileageDueSoonThreshold"/>).
    /// </summary>
    /// <param name="mileageStart">First due mileage (km).</param>
    /// <param name="mileageInterval">Mileage interval (km).</param>
    /// <param name="mileageDueSoonThreshold">Due-soon buffer (km).</param>
    /// <param name="mileageCurrent">Current vehicle mileage (km).</param>
    /// <returns>All due points up to and including the first UPCOMING point.</returns>
    private static List<double> GetMileageDuePoints(double mileageStart, double mileageInterval, double mileageDueSoonThreshold, double mileageCurrent)
    {
        var duePoints = new List<double>();
        double duePoint = mileageStart;
        while (true)
        {
            duePoints.Add(duePoint);
            if (ServiceReminderExtensions.IsUpcomingByMileage(mileageCurrent, duePoint, mileageDueSoonThreshold)) break; // included the first UPCOMING due point
            duePoint += mileageInterval;
        }
        return duePoints;
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
            TimeIntervalValue = scheduleType == ServiceScheduleTypeEnum.TIME ? schedule.TimeIntervalValue : null,
            TimeIntervalUnit = scheduleType == ServiceScheduleTypeEnum.TIME ? schedule.TimeIntervalUnit : null,
            MileageInterval = scheduleType == ServiceScheduleTypeEnum.MILEAGE ? schedule.MileageInterval : null,
            TimeBufferValue = scheduleType == ServiceScheduleTypeEnum.TIME ? schedule.TimeBufferValue : null,
            TimeBufferUnit = scheduleType == ServiceScheduleTypeEnum.TIME ? schedule.TimeBufferUnit : null,
            MileageBuffer = scheduleType == ServiceScheduleTypeEnum.MILEAGE ? schedule.MileageBuffer : null,
            CurrentMileage = vehicle.Mileage,
            MileageVariance = mileageVariance,
            DaysUntilDue = daysUntilDue,
            ScheduleType = scheduleType
        };
    }
}