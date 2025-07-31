using System;

using MediatR;

namespace Application.Features.WorkOrders.Command.CompleteWorkOrder;

public record CompleteWorkOrderCommand(int ID) : IRequest<int> { }