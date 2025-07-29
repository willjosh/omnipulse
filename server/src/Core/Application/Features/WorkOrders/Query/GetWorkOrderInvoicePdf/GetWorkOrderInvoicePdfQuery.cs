using MediatR;

namespace Application.Features.WorkOrders.Query.GetWorkOrderInvoicePdf;

public record GetWorkOrderInvoicePdfQuery(int WorkOrderId) : IRequest<byte[]>;