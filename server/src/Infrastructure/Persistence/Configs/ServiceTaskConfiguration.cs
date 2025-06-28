using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class ServiceTaskConfiguration : IEntityTypeConfiguration<ServiceTask>
{
    public void Configure(EntityTypeBuilder<ServiceTask> builder)
    {
        builder.ToTable("ServiceTasks");
        builder.HasKey(st => st.ID);
        
        // String Length Constraints
        builder.Property(st => st.Name).HasMaxLength(200);
        builder.Property(st => st.Description).HasMaxLength(1000);
        
        // Precision for decimal fields
        builder.Property(st => st.EstimatedCost).HasPrecision(10, 2);
        
        // Unique Constraints
        builder.HasIndex(st => st.Name).IsUnique(); 
        
        // Regular Indexes
        builder.HasIndex(st => st.Category);
        builder.HasIndex(st => st.IsActive);
        builder.HasIndex(st => st.EstimatedLabourHours);
        builder.HasIndex(st => st.EstimatedCost);
        
        // Composite indexes for common queries
        builder.HasIndex(st => new { st.Category, st.IsActive });
        builder.HasIndex(st => new { st.IsActive, st.EstimatedCost });
        
        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceTask_EstimatedLabourHours", 
            "EstimatedLabourHours >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceTask_EstimatedCost", 
            "EstimatedCost >= 0"));
        
    }
}