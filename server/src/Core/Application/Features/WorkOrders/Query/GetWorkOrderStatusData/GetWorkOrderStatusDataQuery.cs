using System;

using MediatR;

namespace Application.Features.WorkOrders.Query.GetWorkOrderStatusData;

public record GetWorkOrderStatusDataQuery : IRequest<GetWorkOrderStatusDataDTO> { }