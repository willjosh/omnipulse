using MediatR;

namespace Application.Features.WorkOrders.Command.DeleteWorkOrder;

public record DeleteWorkOrderCommand(int ID) : IRequest<int> { }