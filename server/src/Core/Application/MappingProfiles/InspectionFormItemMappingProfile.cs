using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

/// <summary>
/// AutoMapper profile for mapping InspectionFormItem-related commands and queries.
/// </summary>
public class InspectionFormItemMappingProfile : Profile
{
    public InspectionFormItemMappingProfile()
    {
        // CreateMap<InspectionFormItem, InspectionFormItemDTO>(MemberList.Destination);
    }
}