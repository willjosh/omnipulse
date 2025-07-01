using System;

using Domain.Entities.Enums;

namespace Domain.Entities;

public class ServiceProgram : BaseEntity
{
    public required string Name { get; set; }
    public required string OEMTag { get; set; }
    public required MeterTypeEnum PrimaryMeterType { get; set; }
    public required MeterTypeEnum SecondaryMeterType { get; set; }
    public required bool IsActive { get; set; } = true;

    // navigation properties
    public required ICollection<ServiceSchedule> ServiceSchedules { get; set; } = [];
}