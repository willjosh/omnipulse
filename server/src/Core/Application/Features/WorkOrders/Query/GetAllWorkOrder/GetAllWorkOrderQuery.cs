using System;

using Application.Features.WorkOrders.Query.GetWorkOrderDetail;
using Application.Models;
using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.WorkOrders.Query.GetAllWorkOrder;

public record GetAllWorkOrderQuery(PaginationParameters Parameters) : IRequest<PagedResult<GetWorkOrderDetailDTO>> { }