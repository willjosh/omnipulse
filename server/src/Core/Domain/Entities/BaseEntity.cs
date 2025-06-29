using System;

namespace Domain.Entities;

public abstract class BaseEntity: IEntity
{
    public int ID { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
