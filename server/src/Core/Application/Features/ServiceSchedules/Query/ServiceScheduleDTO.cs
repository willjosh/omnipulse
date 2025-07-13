using System;
using Application.Features.ServiceTasks.Query.GetAllServiceTask;
using Domain.Entities.Enums;

namespace Application.Features.ServiceSchedules.Query;

public class ServiceScheduleDTO
{
    public required List<GetAllServiceTaskDTO> ServiceTasks { get; set; }
    public required int ID { get; set; }
    public required int ServiceProgramID { get; set; }
    public required string Name { get; set; }
    public int? TimeIntervalValue { get; set; }
    public TimeUnitEnum? TimeIntervalUnit { get; set; }
    public int? TimeBufferValue { get; set; }
    public TimeUnitEnum? TimeBufferUnit { get; set; }
    public int? MileageInterval { get; set; }
    public int? MileageBuffer { get; set; }
    public int? FirstServiceTimeValue { get; set; }
    public TimeUnitEnum? FirstServiceTimeUnit { get; set; }
    public int? FirstServiceMileage { get; set; }
    public required bool IsActive { get; set; }
}