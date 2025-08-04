using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InspectionFormItems.Query.GetAllInspectionFormItem;

using AutoMapper;

using MediatR;

namespace Application.Features.InspectionFormItems.Query.GetInspectionFormItem;

public class GetInspectionQueryHandler : IRequestHandler<GetInspectionFormItemQuery, InspectionFormItemDetailDTO>
{
    private readonly IInspectionFormItemRepository _inspectionFormItemRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllInspectionFormItemQueryHandler> _logger;
    public GetInspectionQueryHandler(IInspectionFormItemRepository inspectionFormItemRepository, IMapper mapper, IAppLogger<GetAllInspectionFormItemQueryHandler> logger)
    {
        _inspectionFormItemRepository = inspectionFormItemRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<InspectionFormItemDetailDTO> Handle(GetInspectionFormItemQuery request, CancellationToken cancellationToken)
    {
        // get vehicle by id
        _logger.LogInformation($"GetInspectionFormItemQueryHandler for InspectionFormItemID: {request.InspectionFormItemID}");
        var inspectionFormItem = await _inspectionFormItemRepository.GetInspectionFormItemDetailsAsync(request.InspectionFormItemID);

        if (inspectionFormItem == null)
        {
            _logger.LogError($"InspectionFormItem with ID {request.InspectionFormItemID} not found.");
            throw new EntityNotFoundException(typeof(InspectionFormItemDetailDTO).ToString(), "InspectionFormItemID", request.InspectionFormItemID.ToString());
        }

        // map to InspectionFormItemDetailDTO
        var inspectionFormItemDetailDto = _mapper.Map<InspectionFormItemDetailDTO>(inspectionFormItem);

        // return InspectionFormItemDetailDTO
        _logger.LogInformation($"Returning InspectionFormItemDetailDTO for InspectionFormItemID: {request.InspectionFormItemID}");
        return inspectionFormItemDetailDto;
    }
}