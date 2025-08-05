using System;

using MediatR;

namespace Application.Features.MaintenanceHistories.Query.GetServiceCostData;

public record GetServiceCostDataQuery : IRequest<GetServiceCostDataDTO> { }