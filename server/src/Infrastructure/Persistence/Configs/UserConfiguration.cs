using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        // String Length Constraints
        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);

        // Regular Indexes
        builder.HasIndex(u => u.FirstName);
        builder.HasIndex(u => u.LastName);
        builder.HasIndex(u => u.HireDate);
        builder.HasIndex(u => u.IsActive);
        builder.HasIndex(u => u.CreatedAt);

        // Composite indexes for common queries
        builder.HasIndex(u => new { u.FirstName, u.LastName });
        builder.HasIndex(u => new { u.IsActive, u.HireDate });
        builder.HasIndex(u => new { u.IsActive, u.Email });

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_User_HireDate",
            "HireDate >= '1900-01-01' AND HireDate <= GETDATE()"));
        builder.ToTable(t => t.HasCheckConstraint("CK_User_Timestamps",
            "UpdatedAt >= CreatedAt"));
    }
}
