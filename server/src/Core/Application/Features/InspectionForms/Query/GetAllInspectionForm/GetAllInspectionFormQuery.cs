using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.InspectionForms.Query.GetAllInspectionForm;

public record GetAllInspectionFormQuery(PaginationParameters Parameters) : IRequest<PagedResult<InspectionFormDTO>>;