using System;

namespace Domain.Entities;

public class MonthlyCostData
{
    /// <summary>
    /// Month in YYYY-MM format for easy sorting and processing.
    /// </summary>
    /// <example>2025-01</example>
    public required string Month { get; set; }

    /// <summary>
    /// Human-readable month name for display purposes.
    /// </summary>
    /// <example>January 2025</example>
    public required string MonthName { get; set; }

    /// <summary>
    /// Total service cost for this month.
    /// </summary>
    /// <example>19450.80</example>
    public required decimal TotalCost { get; set; }

    /// <summary>
    /// Number of service records in this month.
    /// </summary>
    /// <example>15</example>
    public required int ServiceCount { get; set; }
}