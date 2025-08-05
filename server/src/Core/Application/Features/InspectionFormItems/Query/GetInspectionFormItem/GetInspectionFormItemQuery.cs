using Application.Features.InspectionFormItems.Query.GetAllInspectionFormItem;

using MediatR;

namespace Application.Features.InspectionFormItems.Query.GetInspectionFormItem;

public record GetInspectionFormItemQuery(int InspectionFormItemID) : IRequest<InspectionFormItemDetailDTO> { }