using Application.Features.ServiceTasks.Query;

using Domain.Entities.Enums;

namespace Application.Features.ServiceSchedules.Query;

public class ServiceScheduleDTO
{
    public required List<ServiceTaskDTO> ServiceTasks { get; set; }

    /// <example>1</example>
    public required int ID { get; set; }

    /// <example>1</example>
    public required int ServiceProgramID { get; set; }

    /// <example>Service Schedule Name</example>
    public required string Name { get; set; }

    /// <example>7</example>
    public int? TimeIntervalValue { get; set; }

    /// <example>2</example>
    public TimeUnitEnum? TimeIntervalUnit { get; set; }

    /// <example>3</example>
    public int? TimeBufferValue { get; set; }

    /// <example>2</example>
    public TimeUnitEnum? TimeBufferUnit { get; set; }

    /// <example>1000</example>
    public int? MileageInterval { get; set; }

    /// <example>100</example>
    public int? MileageBuffer { get; set; }

    public DateTime? FirstServiceDate { get; set; }

    /// <example>1000</example>
    public int? FirstServiceMileage { get; set; }

    /// <example>true</example>
    public required bool IsActive { get; set; }
}