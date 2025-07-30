using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(i => i.WorkOrderID);

        builder.Property(i => i.InvoiceNumber).HasMaxLength(50);
        builder.Property(i => i.PaymentMethod).HasMaxLength(50);

        builder.Property(i => i.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(i => i.TaxAmount).HasColumnType("decimal(18,2)");

        builder.HasIndex(i => i.InvoiceNumber).IsUnique();

        builder.HasIndex(i => i.GeneratedByUserID);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.InvoiceDate);
        builder.HasIndex(i => i.PaymentDate);

        builder.ToTable(t => t.HasCheckConstraint("CK_Invoice_TotalAmount", "TotalAmount >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_Invoice_TaxAmount", "TaxAmount >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_Invoice_PaymentDate", "PaymentDate IS NULL OR PaymentDate >= InvoiceDate"));

        // Table Relationships
        builder
            .HasOne(i => i.WorkOrder)
            .WithMany(w => w.Invoices)
            .HasForeignKey(i => i.WorkOrderID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(i => i.User)
            .WithMany()
            .HasForeignKey(i => i.GeneratedByUserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}