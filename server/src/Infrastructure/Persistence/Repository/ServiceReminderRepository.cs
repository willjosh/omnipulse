
using Application.Contracts.Persistence;
using Application.Features.ServiceReminders.Query;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class ServiceReminderRepository : GenericRepository<ServiceReminder>, IServiceReminderRepository
{
    // Configuration constants to avoid hard-coded values
    private const int MaxOccurrenceCount = 100;
    private const int DefaultMileageBufferKm = 1000;
    private const double DefaultDailyMileageEstimate = 30.0; // km/day for time vs mileage comparison
    private const int DaysPerWeek = 7;

    private readonly OmnipulseDatabaseContext _dbContext;

    public ServiceReminderRepository(OmnipulseDatabaseContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<PagedResult<ServiceReminderDTO>> GetAllCalculatedServiceRemindersPagedAsync(PaginationParameters parameters)
    {
        // Generate all calculated service reminders
        var allReminders = await GenerateCalculatedServiceRemindersAsync();

        // Apply search filter if provided
        var filteredReminders = ApplySearchFilter(allReminders, parameters.Search);

        // Apply sorting
        var sortedReminders = ApplySorting(filteredReminders, parameters.SortBy, parameters.SortDescending);

        // Apply pagination
        var totalCount = sortedReminders.Count;
        var pagedReminders = sortedReminders
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToList();

        return new PagedResult<ServiceReminderDTO>
        {
            Items = pagedReminders,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static List<ServiceReminderDTO> ApplySearchFilter(List<ServiceReminderDTO> reminders, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return reminders;

        var searchText = search.Trim().ToLowerInvariant();
        return reminders.Where(r =>
            r.ServiceScheduleName.ToLowerInvariant().Contains(searchText) ||
            (r.ServiceProgramName?.ToLowerInvariant().Contains(searchText) ?? false) ||
            r.VehicleName.ToLowerInvariant().Contains(searchText) ||
            r.ServiceTasks.Any(t => t.ServiceTaskName.ToLowerInvariant().Contains(searchText))
        ).ToList();
    }

    private static List<ServiceReminderDTO> ApplySorting(List<ServiceReminderDTO> reminders, string? sortBy, bool sortDescending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            // Default sorting: Status priority (overdue first), then by due date/mileage
            return reminders.OrderBy(r => GetStatusPriority(r.Status))
                           .ThenBy(r => r.DueDate ?? DateTime.MaxValue)
                           .ThenBy(r => r.DueMileage ?? double.MaxValue)
                           .ToList();
        }

        var sortField = sortBy.ToLowerInvariant();

        return sortField switch
        {
            "vehiclename" => sortDescending
                ? reminders.OrderByDescending(r => r.VehicleName).ToList()
                : reminders.OrderBy(r => r.VehicleName).ToList(),
            "servicetaskname" => sortDescending
                ? reminders.OrderByDescending(r => r.ServiceScheduleName).ToList()
                : reminders.OrderBy(r => r.ServiceScheduleName).ToList(),
            "duedate" => sortDescending
                ? reminders.OrderByDescending(r => r.DueDate ?? DateTime.MinValue).ToList()
                : reminders.OrderBy(r => r.DueDate ?? DateTime.MaxValue).ToList(),
            "duemileage" => sortDescending
                ? reminders.OrderByDescending(r => r.DueMileage ?? double.MinValue).ToList()
                : reminders.OrderBy(r => r.DueMileage ?? double.MaxValue).ToList(),
            "status" => sortDescending
                ? reminders.OrderByDescending(r => r.Status).ToList()
                : reminders.OrderBy(r => r.Status).ToList(),
            "prioritylevel" => sortDescending
                ? reminders.OrderByDescending(r => r.PriorityLevel).ToList()
                : reminders.OrderBy(r => r.PriorityLevel).ToList(),
            "occurrencenumber" => sortDescending
                ? reminders.OrderByDescending(r => r.OccurrenceNumber).ToList()
                : reminders.OrderBy(r => r.OccurrenceNumber).ToList(),
            _ => reminders.OrderBy(r => GetStatusPriority(r.Status)).ToList()
        };
    }

    private static int GetStatusPriority(ServiceReminderStatusEnum status) => status switch
    {
        ServiceReminderStatusEnum.OVERDUE => 0,
        ServiceReminderStatusEnum.DUE_SOON => 1,
        _ => 2
    };

    public async Task<ServiceReminder?> GetServiceReminderWithDetailsAsync(int serviceReminderId)
    {
        return await _dbSet
            .Include(sr => sr.Vehicle)
            .Include(sr => sr.ServiceProgram)
            .Include(sr => sr.ServiceSchedule)

            .Include(sr => sr.WorkOrder)
            .FirstOrDefaultAsync(sr => sr.ID == serviceReminderId);
    }

    // Query methods by vehicle
    public async Task<IReadOnlyList<ServiceReminder>> GetRemindersByVehicleIdAsync(int vehicleId)
    {
        return await _dbSet

            .Include(sr => sr.ServiceSchedule)
            .Where(sr => sr.VehicleID == vehicleId)
            .OrderBy(sr => sr.Status == ServiceReminderStatusEnum.OVERDUE ? 0 :
                sr.Status == ServiceReminderStatusEnum.DUE_SOON ? 1 : 2)
            .ThenBy(sr => sr.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ServiceReminder>> GetOverdueRemindersByVehicleIdAsync(int vehicleId)
    {
        return await _dbSet

            .Include(sr => sr.ServiceSchedule)
            .Where(sr => sr.VehicleID == vehicleId && sr.Status == ServiceReminderStatusEnum.OVERDUE)
            .OrderBy(sr => sr.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ServiceReminder>> GetUpcomingRemindersByVehicleIdAsync(int vehicleId)
    {
        return await _dbSet

            .Include(sr => sr.ServiceSchedule)
            .Where(sr => sr.VehicleID == vehicleId &&
                (sr.Status == ServiceReminderStatusEnum.DUE_SOON || sr.Status == ServiceReminderStatusEnum.UPCOMING))
            .OrderBy(sr => sr.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    // Query methods by status
    public async Task<IReadOnlyList<ServiceReminder>> GetRemindersByStatusAsync(ServiceReminderStatusEnum status)
    {
        return await _dbSet
            .Include(sr => sr.Vehicle)

            .Where(sr => sr.Status == status)
            .OrderBy(sr => sr.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ServiceReminder>> GetRemindersByPriorityAsync(PriorityLevelEnum priority)
    {
        return await _dbSet
            .Include(sr => sr.Vehicle)

            .Where(sr => sr.PriorityLevel == priority)
            .OrderBy(sr => sr.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    // Query methods by service schedule
    public async Task<IReadOnlyList<ServiceReminder>> GetRemindersByServiceScheduleIdAsync(int serviceScheduleId)
    {
        return await _dbSet
            .Include(sr => sr.Vehicle)

            .Where(sr => sr.ServiceScheduleID == serviceScheduleId)
            .OrderBy(sr => sr.VehicleID)
            .ThenBy(sr => sr.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    // TODO: This method needs to be redesigned for schedule-based reminders
    // public async Task<IReadOnlyList<ServiceReminder>> GetRemindersByServiceTaskIdAsync(int serviceTaskId)

    // Query methods by date ranges
    public async Task<IReadOnlyList<ServiceReminder>> GetRemindersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(sr => sr.Vehicle)

            .Where(sr => sr.DueDate.HasValue && sr.DueDate.Value >= startDate && sr.DueDate.Value <= endDate)
            .OrderBy(sr => sr.DueDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ServiceReminder>> GetRemindersCompletedInDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(sr => sr.Vehicle)

            .Where(sr => sr.Status == ServiceReminderStatusEnum.COMPLETED &&
                sr.CompletedDate.HasValue &&
                sr.CompletedDate.Value >= startDate &&
                sr.CompletedDate.Value <= endDate)
            .OrderBy(sr => sr.CompletedDate)
            .ToListAsync();
    }

    // Statistics methods
    public async Task<int> CountRemindersByVehicleIdAsync(int vehicleId)
    {
        return await _dbSet.CountAsync(sr => sr.VehicleID == vehicleId);
    }

    public async Task<int> CountOverdueRemindersByVehicleIdAsync(int vehicleId)
    {
        return await _dbSet.CountAsync(sr => sr.VehicleID == vehicleId && sr.Status == ServiceReminderStatusEnum.OVERDUE);
    }

    public async Task<int> CountRemindersByStatusAsync(ServiceReminderStatusEnum status)
    {
        return await _dbSet.CountAsync(sr => sr.Status == status);
    }

    // Business logic methods
    public async Task<bool> HasPendingRemindersForVehicleAsync(int vehicleId)
    {
        return await _dbSet.AnyAsync(sr => sr.VehicleID == vehicleId &&
                                    sr.Status != ServiceReminderStatusEnum.COMPLETED &&
                                    sr.Status != ServiceReminderStatusEnum.CANCELLED);
    }

    public async Task<bool> HasOverdueRemindersForVehicleAsync(int vehicleId)
    {
        return await _dbSet.AnyAsync(sr => sr.VehicleID == vehicleId && sr.Status == ServiceReminderStatusEnum.OVERDUE);
    }

    public async Task<ServiceReminder?> GetNextDueReminderForVehicleAsync(int vehicleId)
    {
        return await _dbSet

            .Include(sr => sr.ServiceSchedule)
            .Where(sr => sr.VehicleID == vehicleId &&
                sr.Status != ServiceReminderStatusEnum.COMPLETED &&
                sr.Status != ServiceReminderStatusEnum.CANCELLED &&
                sr.DueDate.HasValue)
            .OrderBy(sr => sr.DueDate)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Generates all calculated service reminders for all active schedules and vehicles.
    /// This is the main complex logic that calculates dynamic reminders.
    /// </summary>
    private async Task<List<ServiceReminderDTO>> GenerateCalculatedServiceRemindersAsync()
    {
        var currentDate = DateTime.UtcNow;
        var reminders = new List<ServiceReminderDTO>();

        // Get all data in a single query with proper includes to avoid N+1 problem
        var serviceSchedulesWithData = await _dbContext.Set<ServiceSchedule>()
            .Where(ss => ss.IsActive)
            .Include(ss => ss.ServiceProgram)
                .ThenInclude(sp => sp.XrefServiceProgramVehicles)
                .ThenInclude(xspv => xspv.Vehicle)
            .Include(ss => ss.XrefServiceScheduleServiceTasks)
                .ThenInclude(xsst => xsst.ServiceTask)
            .ToListAsync();

        foreach (var schedule in serviceSchedulesWithData)
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

        return reminders;
    }



    private List<ServiceReminderDTO> GenerateRemindersForSchedule(
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

    private List<ServiceReminderDTO> GenerateOverdueAndDueSoonReminders(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> serviceTasks,
        DateTime assignmentDate,
        DateTime currentDate)
    {
        var reminders = new List<ServiceReminderDTO>();

        // Generate time-based overdue and due soon reminders
        if (schedule.TimeIntervalValue.HasValue && schedule.TimeIntervalUnit.HasValue)
        {
            var overdueReminders = GenerateTimeBasedOverdueReminders(
                schedule, vehicle, serviceTasks, assignmentDate, currentDate);
            reminders.AddRange(overdueReminders);

            var dueSoonReminders = GenerateTimeBasedDueSoonReminders(
                schedule, vehicle, serviceTasks, assignmentDate, currentDate);
            reminders.AddRange(dueSoonReminders);
        }

        // Generate mileage-based overdue and due soon reminders
        if (schedule.MileageInterval.HasValue)
        {
            var overdueMileageReminders = GenerateMileageBasedOverdueReminders(
                schedule, vehicle, serviceTasks, assignmentDate, currentDate);
            reminders.AddRange(overdueMileageReminders);

            var dueSoonMileageReminders = GenerateMileageBasedDueSoonReminders(
                schedule, vehicle, serviceTasks, assignmentDate, currentDate);
            reminders.AddRange(dueSoonMileageReminders);
        }

        return reminders;
    }

    private List<ServiceReminderDTO> GenerateTimeBasedOverdueReminders(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> serviceTasks,
        DateTime assignmentDate,
        DateTime currentDate)
    {
        var reminders = new List<ServiceReminderDTO>();

        // Determine start date for calculations
        var startDate = schedule.FirstServiceDate ?? assignmentDate;

        // Generate reminders for each occurrence until we find all overdue reminders
        var occurrenceNumber = 1;
        var currentOccurrenceDate = startDate;

        while (currentOccurrenceDate <= currentDate.AddDays(365)) // Look ahead 1 year for overdue reminders
        {
            var status = CalculateTimeBasedStatus(currentOccurrenceDate, currentDate, schedule);

            // Only include overdue reminders
            if (status == ServiceReminderStatusEnum.OVERDUE)
            {
                var reminder = CreateServiceReminderDTO(
                    schedule, vehicle, serviceTasks, currentOccurrenceDate, null,
                    status, occurrenceNumber, isTimeBasedReminder: true,
                    isMileageBasedReminder: false);

                reminders.Add(reminder);
            }

            // Calculate next occurrence
            currentOccurrenceDate = AddTimeInterval(
                currentOccurrenceDate,
                schedule.TimeIntervalValue!.Value,
                schedule.TimeIntervalUnit!.Value);
            occurrenceNumber++;

            // Safety check to prevent infinite loops
            if (occurrenceNumber > MaxOccurrenceCount)
            {
                break;
            }
        }

        return reminders;
    }

    private List<ServiceReminderDTO> GenerateTimeBasedDueSoonReminders(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> serviceTasks,
        DateTime assignmentDate,
        DateTime currentDate)
    {
        var reminders = new List<ServiceReminderDTO>();

        // Determine start date for calculations
        var startDate = schedule.FirstServiceDate ?? assignmentDate;

        // Generate reminders for each occurrence until we find all due soon reminders
        var occurrenceNumber = 1;
        var currentOccurrenceDate = startDate;

        while (currentOccurrenceDate <= currentDate.AddDays(365)) // Look ahead 1 year for due soon reminders
        {
            var status = CalculateTimeBasedStatus(currentOccurrenceDate, currentDate, schedule);

            // Only include due soon reminders
            if (status == ServiceReminderStatusEnum.DUE_SOON)
            {
                var reminder = CreateServiceReminderDTO(
                    schedule, vehicle, serviceTasks, currentOccurrenceDate, null,
                    status, occurrenceNumber, isTimeBasedReminder: true,
                    isMileageBasedReminder: false);

                reminders.Add(reminder);
            }

            // Calculate next occurrence
            currentOccurrenceDate = AddTimeInterval(
                currentOccurrenceDate,
                schedule.TimeIntervalValue!.Value,
                schedule.TimeIntervalUnit!.Value);
            occurrenceNumber++;

            // Safety check to prevent infinite loops
            if (occurrenceNumber > MaxOccurrenceCount)
            {
                break;
            }
        }

        return reminders;
    }

    private List<ServiceReminderDTO> GenerateMileageBasedOverdueReminders(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> serviceTasks,
        DateTime assignmentDate,
        DateTime currentDate)
    {
        var reminders = new List<ServiceReminderDTO>();

        // Determine start mileage for calculations
        var startMileage = schedule.FirstServiceMileage ?? vehicle.Mileage;

        // Generate reminders for each mileage occurrence
        var occurrenceNumber = 1;
        var currentOccurrenceMileage = startMileage;

        // Calculate how far ahead to look based on buffer
        var mileageBuffer = schedule.MileageBuffer ?? DefaultMileageBufferKm;
        var maxMileage = vehicle.Mileage + mileageBuffer;

        while (currentOccurrenceMileage <= maxMileage)
        {
            var status = CalculateMileageBasedStatus(currentOccurrenceMileage, vehicle.Mileage, schedule);

            // Only include overdue reminders
            if (status == ServiceReminderStatusEnum.OVERDUE)
            {
                var reminder = CreateServiceReminderDTO(
                    schedule, vehicle, serviceTasks, null, currentOccurrenceMileage,
                    status, occurrenceNumber, isTimeBasedReminder: false,
                    isMileageBasedReminder: true);

                reminders.Add(reminder);
            }

            // Calculate next occurrence
            currentOccurrenceMileage += schedule.MileageInterval!.Value;
            occurrenceNumber++;

            // Safety check to prevent infinite loops
            if (occurrenceNumber > MaxOccurrenceCount)
            {
                break;
            }
        }

        return reminders;
    }

    private List<ServiceReminderDTO> GenerateMileageBasedDueSoonReminders(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> serviceTasks,
        DateTime assignmentDate,
        DateTime currentDate)
    {
        var reminders = new List<ServiceReminderDTO>();

        // Determine start mileage for calculations
        var startMileage = schedule.FirstServiceMileage ?? vehicle.Mileage;

        // Generate reminders for each mileage occurrence
        var occurrenceNumber = 1;
        var currentOccurrenceMileage = startMileage;

        // Calculate how far ahead to look based on buffer
        var mileageBuffer = schedule.MileageBuffer ?? DefaultMileageBufferKm;
        var maxMileage = vehicle.Mileage + mileageBuffer;

        while (currentOccurrenceMileage <= maxMileage)
        {
            var status = CalculateMileageBasedStatus(currentOccurrenceMileage, vehicle.Mileage, schedule);

            // Only include due soon reminders
            if (status == ServiceReminderStatusEnum.DUE_SOON)
            {
                var reminder = CreateServiceReminderDTO(
                    schedule, vehicle, serviceTasks, null, currentOccurrenceMileage,
                    status, occurrenceNumber, isTimeBasedReminder: false,
                    isMileageBasedReminder: true);

                reminders.Add(reminder);
            }

            // Calculate next occurrence
            currentOccurrenceMileage += schedule.MileageInterval!.Value;
            occurrenceNumber++;

            // Safety check to prevent infinite loops
            if (occurrenceNumber > MaxOccurrenceCount)
            {
                break;
            }
        }

        return reminders;
    }

    private ServiceReminderDTO? GenerateNextUpcomingReminder(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> serviceTasks,
        DateTime assignmentDate,
        DateTime currentDate)
    {
        DateTime? nextTimeOccurrence = null;
        double? nextMileageOccurrence = null;
        int timeOccurrenceNumber = 0;
        int mileageOccurrenceNumber = 0;

        // Calculate next time-based occurrence
        if (schedule.TimeIntervalValue.HasValue && schedule.TimeIntervalUnit.HasValue)
        {
            var startDate = schedule.FirstServiceDate ?? assignmentDate;
            var currentOccurrenceDate = startDate;
            var occurrenceNumber = 1;

            // Find the next occurrence that's truly upcoming (not overdue or due soon)
            while (occurrenceNumber <= MaxOccurrenceCount)
            {
                // Check if this occurrence is in the future and outside the buffer
                if (currentOccurrenceDate > currentDate)
                {
                    var status = CalculateTimeBasedStatus(currentOccurrenceDate, currentDate, schedule);

                    if (status == ServiceReminderStatusEnum.UPCOMING)
                    {
                        nextTimeOccurrence = currentOccurrenceDate;
                        timeOccurrenceNumber = occurrenceNumber;
                        break;
                    }
                }

                currentOccurrenceDate = AddTimeInterval(
                    currentOccurrenceDate,
                    schedule.TimeIntervalValue.Value,
                    schedule.TimeIntervalUnit.Value);
                occurrenceNumber++;
            }
        }

        // Calculate next mileage-based occurrence
        if (schedule.MileageInterval.HasValue)
        {
            var startMileage = schedule.FirstServiceMileage ?? vehicle.Mileage;
            var currentOccurrenceMileage = startMileage;
            var occurrenceNumber = 1;
            var mileageBuffer = schedule.MileageBuffer ?? DefaultMileageBufferKm;
            var maxMileage = vehicle.Mileage + mileageBuffer;

            // Find the next occurrence that's truly upcoming (not overdue or due soon)
            while (currentOccurrenceMileage <= maxMileage && occurrenceNumber <= MaxOccurrenceCount)
            {
                // Check if this occurrence is in the future and outside the buffer
                if (currentOccurrenceMileage > vehicle.Mileage)
                {
                    var status = CalculateMileageBasedStatus(currentOccurrenceMileage, vehicle.Mileage, schedule);
                    if (status == ServiceReminderStatusEnum.UPCOMING)
                    {
                        nextMileageOccurrence = currentOccurrenceMileage;
                        mileageOccurrenceNumber = occurrenceNumber;
                        break;
                    }
                }

                currentOccurrenceMileage += schedule.MileageInterval.Value;
                occurrenceNumber++;
            }
        }

        // Determine which comes first and create the appropriate reminder
        if (nextTimeOccurrence.HasValue && nextMileageOccurrence.HasValue)
        {
            // Both time and mileage are provided - determine which comes first
            // For simplicity, we'll use a more realistic approach: calculate estimated days until each
            var timeDaysUntilDue = (nextTimeOccurrence.Value - currentDate).TotalDays;

            // Estimate days until mileage based on current vehicle usage
            // Use a conservative estimate for comparison
            var mileageDaysUntilDue = (nextMileageOccurrence.Value - vehicle.Mileage) / DefaultDailyMileageEstimate;

            if (timeDaysUntilDue <= mileageDaysUntilDue)
            {
                // Time comes first
                return CreateServiceReminderDTO(
                    schedule, vehicle, serviceTasks, nextTimeOccurrence.Value, null,
                    ServiceReminderStatusEnum.UPCOMING, timeOccurrenceNumber,
                    isTimeBasedReminder: true, isMileageBasedReminder: false);
            }
            else
            {
                // Mileage comes first
                return CreateServiceReminderDTO(
                    schedule, vehicle, serviceTasks, null, nextMileageOccurrence.Value,
                    ServiceReminderStatusEnum.UPCOMING, mileageOccurrenceNumber,
                    isTimeBasedReminder: false, isMileageBasedReminder: true);
            }
        }
        else if (nextTimeOccurrence.HasValue)
        {
            // Only time-based
            return CreateServiceReminderDTO(
                schedule, vehicle, serviceTasks, nextTimeOccurrence.Value, null,
                ServiceReminderStatusEnum.UPCOMING, timeOccurrenceNumber,
                isTimeBasedReminder: true, isMileageBasedReminder: false);
        }
        else if (nextMileageOccurrence.HasValue)
        {
            // Only mileage-based
            return CreateServiceReminderDTO(
                schedule, vehicle, serviceTasks, null, nextMileageOccurrence.Value,
                ServiceReminderStatusEnum.UPCOMING, mileageOccurrenceNumber,
                isTimeBasedReminder: false, isMileageBasedReminder: true);
        }

        // No upcoming reminders found
        return null;
    }

    private ServiceReminderDTO CreateServiceReminderDTO(
        ServiceSchedule schedule,
        Vehicle vehicle,
        List<ServiceTask> tasks,
        DateTime? dueDate,
        double? dueMileage,
        ServiceReminderStatusEnum status,
        int occurrenceNumber,
        bool isTimeBasedReminder,
        bool isMileageBasedReminder)
    {
        var currentDate = DateTime.UtcNow;

        // Create ServiceTaskInfo DTOs for all tasks
        var serviceTasks = tasks.Select(task => new ServiceTaskInfoDTO
        {
            ServiceTaskID = task.ID,
            ServiceTaskName = task.Name,
            ServiceTaskCategory = task.Category,
            EstimatedLabourHours = task.EstimatedLabourHours,
            EstimatedCost = task.EstimatedCost,
            Description = task.Description,
            IsRequired = true // Assuming all tasks in a schedule are required
        }).ToList();

        return new ServiceReminderDTO
        {
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
            PriorityLevel = CalculatePriorityLevel(status),
            TimeIntervalValue = schedule.TimeIntervalValue,
            TimeIntervalUnit = schedule.TimeIntervalUnit,
            MileageInterval = schedule.MileageInterval,
            TimeBufferValue = schedule.TimeBufferValue,
            TimeBufferUnit = schedule.TimeBufferUnit,
            MileageBuffer = schedule.MileageBuffer,
            CurrentMileage = vehicle.Mileage,
            MileageVariance = dueMileage.HasValue ? vehicle.Mileage - dueMileage.Value : null,
            DaysUntilDue = dueDate.HasValue ? (int)(dueDate.Value - currentDate).TotalDays : null,
            OccurrenceNumber = occurrenceNumber,
            IsTimeBasedReminder = isTimeBasedReminder,
            IsMileageBasedReminder = isMileageBasedReminder
        };
    }

    private static ServiceReminderStatusEnum CalculateTimeBasedStatus(
        DateTime dueDate,
        DateTime currentDate,
        ServiceSchedule schedule)
    {
        if (currentDate > dueDate)
            return ServiceReminderStatusEnum.OVERDUE;

        if (schedule.TimeBufferValue.HasValue && schedule.TimeBufferUnit.HasValue)
        {
            var dueSoonThreshold = dueDate.AddDays(-ConvertTodays(schedule.TimeBufferValue.Value, schedule.TimeBufferUnit.Value));
            if (currentDate >= dueSoonThreshold)
                return ServiceReminderStatusEnum.DUE_SOON;
        }

        return ServiceReminderStatusEnum.UPCOMING;
    }

    private static ServiceReminderStatusEnum CalculateMileageBasedStatus(
        double dueMileage,
        double currentMileage,
        ServiceSchedule schedule)
    {
        if (currentMileage > dueMileage)
            return ServiceReminderStatusEnum.OVERDUE;

        if (schedule.MileageBuffer.HasValue)
        {
            var dueSoonThreshold = dueMileage - schedule.MileageBuffer.Value;
            if (currentMileage >= dueSoonThreshold)
                return ServiceReminderStatusEnum.DUE_SOON;
        }

        return ServiceReminderStatusEnum.UPCOMING;
    }

    private static PriorityLevelEnum CalculatePriorityLevel(ServiceReminderStatusEnum status)
    {
        return status switch
        {
            ServiceReminderStatusEnum.OVERDUE => PriorityLevelEnum.HIGH,
            ServiceReminderStatusEnum.DUE_SOON => PriorityLevelEnum.MEDIUM,
            ServiceReminderStatusEnum.UPCOMING => PriorityLevelEnum.LOW,
            _ => PriorityLevelEnum.LOW
        };
    }

    private static DateTime AddTimeInterval(DateTime date, int value, TimeUnitEnum unit)
    {
        return unit switch
        {
            TimeUnitEnum.Hours => date.AddHours(value),
            TimeUnitEnum.Days => date.AddDays(value),
            TimeUnitEnum.Weeks => date.AddDays(value * DaysPerWeek),
            _ => throw new ArgumentException($"Unsupported time unit: {unit}")
        };
    }

    private static int ConvertTodays(int value, TimeUnitEnum unit)
    {
        return unit switch
        {
            TimeUnitEnum.Hours => (int)Math.Ceiling(value / 24.0),
            TimeUnitEnum.Days => value,
            TimeUnitEnum.Weeks => value * DaysPerWeek,
            _ => throw new ArgumentException($"Unsupported time unit: {unit}")
        };
    }

    public async Task<IReadOnlyList<ServiceReminder>> GetServiceRemindersByWorkOrderIdAsync(int workOrderID)
    {
        return await _dbSet.Where(sr => sr.WorkOrderID == workOrderID).ToListAsync();
    }

    public async Task<PagedResult<ServiceReminder>> GetAllServiceRemindersPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        // Include related data
        query = query
            .Include(sr => sr.Vehicle)
            .Include(sr => sr.ServiceProgram)
            .Include(sr => sr.ServiceSchedule)
            .Include(sr => sr.WorkOrder);

        // Apply search filter
        query = ApplySearchFilter(query, parameters.Search);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<ServiceReminder>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<ServiceReminder> ApplySearchFilter(IQueryable<ServiceReminder> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return query;

        string searchPattern = $"%{searchText.Trim().ToLowerInvariant()}%";

        return query.Where(sr =>
            EF.Functions.Like(sr.ServiceScheduleName, searchPattern) ||
            EF.Functions.Like(sr.ServiceProgramName ?? string.Empty, searchPattern) ||
            EF.Functions.Like(sr.Vehicle.Name, searchPattern) ||
            EF.Functions.Like(sr.Vehicle.Make, searchPattern) ||
            EF.Functions.Like(sr.Vehicle.Model, searchPattern) ||
            EF.Functions.Like(sr.Vehicle.LicensePlate, searchPattern)
        );
    }

    private static IQueryable<ServiceReminder> ApplySorting(IQueryable<ServiceReminder> query, string? sortBy, bool sortDescending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderBy(sr => sr.Status == ServiceReminderStatusEnum.OVERDUE ? 0 :
                              sr.Status == ServiceReminderStatusEnum.DUE_SOON ? 1 : 2)
                        .ThenBy(sr => sr.DueDate ?? DateTime.MaxValue); // Default sort: overdue first, then by due date
        }

        var sortByLower = sortBy.ToLower();
        return sortByLower switch
        {
            "vehiclename" => sortDescending
                ? query.OrderByDescending(sr => sr.Vehicle.Name)
                : query.OrderBy(sr => sr.Vehicle.Name),

            "serviceschedulename" => sortDescending
                ? query.OrderByDescending(sr => sr.ServiceScheduleName)
                : query.OrderBy(sr => sr.ServiceScheduleName),
            "serviceprogramname" => sortDescending
                ? query.OrderByDescending(sr => sr.ServiceProgramName)
                : query.OrderBy(sr => sr.ServiceProgramName),
            "duedate" => sortDescending
                ? query.OrderByDescending(sr => sr.DueDate)
                : query.OrderBy(sr => sr.DueDate),
            "duemileage" => sortDescending
                ? query.OrderByDescending(sr => sr.DueMileage)
                : query.OrderBy(sr => sr.DueMileage),
            "status" => sortDescending
                ? query.OrderByDescending(sr => sr.Status)
                : query.OrderBy(sr => sr.Status),
            "prioritylevel" => sortDescending
                ? query.OrderByDescending(sr => sr.PriorityLevel)
                : query.OrderBy(sr => sr.PriorityLevel),
            "createdat" => sortDescending
                ? query.OrderByDescending(sr => sr.CreatedAt)
                : query.OrderBy(sr => sr.CreatedAt),
            "updatedat" => sortDescending
                ? query.OrderByDescending(sr => sr.UpdatedAt)
                : query.OrderBy(sr => sr.UpdatedAt),
            _ => query.OrderBy(sr => sr.Status == ServiceReminderStatusEnum.OVERDUE ? 0 :
                              sr.Status == ServiceReminderStatusEnum.DUE_SOON ? 1 : 2)
                      .ThenBy(sr => sr.DueDate ?? DateTime.MaxValue) // Default sort for unrecognized fields
        };
    }
}