using System;

namespace Domain.Entities;

public abstract class BaseEntity
{
    public required int ID { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}