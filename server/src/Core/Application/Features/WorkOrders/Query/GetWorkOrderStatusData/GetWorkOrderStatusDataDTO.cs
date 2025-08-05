using System;

namespace Application.Features.WorkOrders.Query.GetWorkOrderStatusData;

public class GetWorkOrderStatusDataDTO
{
    public int CreatedCount { get; set; }
    public int InProgressCount { get; set; }
}