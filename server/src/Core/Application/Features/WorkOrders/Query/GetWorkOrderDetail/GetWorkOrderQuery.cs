using Application.Models;
using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.WorkOrders.Query.GetWorkOrderDetail;

public record GetWorkOrderDetailQuery(int ID) : IRequest<GetWorkOrderDetailDTO> { }