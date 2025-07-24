using System;

namespace Domain.Entities;

public abstract class BaseEntity
{
    public int ID { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}