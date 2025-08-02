using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

namespace Application.Features.InspectionFormItems.Query.GetAllInspectionFormItem;

/// <summary>
/// Query for retrieving all <see cref="InspectionFormItem"/> for a specific <see cref="InspectionForm"/> with pagination.
/// </summary>
/// <param name="InspectionFormID">The ID of the <see cref="InspectionForm"/> to get items for.</param>
/// <param name="Parameters">Pagination, filtering, and sorting parameters.</param>
public record GetAllInspectionFormItemQuery(int InspectionFormID, PaginationParameters Parameters) : IRequest<PagedResult<GetAllInspectionFormItemDTO>>;