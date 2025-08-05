using Domain.Entities;

namespace Application.Features.MaintenanceHistories.Query.GetServiceCostData;

public class GetServiceCostDataDTO
{
    /// <summary>
    /// Monthly service cost data for the last 6 months, ordered from oldest to newest.
    /// </summary>
    /// <example>[
    ///   { "Month": "2024-08", "MonthName": "August 2024", "TotalCost": 15420.50 },
    ///   { "Month": "2024-09", "MonthName": "September 2024", "TotalCost": 18750.25 },
    ///   { "Month": "2024-10", "MonthName": "October 2024", "TotalCost": 12300.00 },
    ///   { "Month": "2024-11", "MonthName": "November 2024", "TotalCost": 22100.75 },
    ///   { "Month": "2024-12", "MonthName": "December 2024", "TotalCost": 16800.30 },
    ///   { "Month": "2025-01", "MonthName": "January 2025", "TotalCost": 19450.80 }
    /// ]</example>
    public required List<MonthlyCostData> MonthlyCosts { get; set; } = [];

    /// <summary>
    /// Total cost across all 6 months.
    /// </summary>
    /// <example>104821.60</example>
    public required decimal TotalCostForPeriod { get; set; }

    /// <summary>
    /// Average monthly cost over the 6-month period.
    /// </summary>
    /// <example>17470.27</example>
    public required decimal AverageMonthlyCost { get; set; }
}