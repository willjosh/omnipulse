using MediatR;

namespace Application.Features.WorkOrders.Query.GetWorkOrderInvoice;

public record GetWorkOrderInvoiceQuery(int WorkOrderId) : IRequest<GetWorkOrderInvoiceDTO>;