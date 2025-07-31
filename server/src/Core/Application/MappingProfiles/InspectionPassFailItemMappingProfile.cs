using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

/// <summary>
/// AutoMapper profile for mapping InspectionPassFailItem-related commands and queries.
/// </summary>
public class InspectionPassFailItemMappingProfile : Profile
{
    public InspectionPassFailItemMappingProfile()
    {
        // CreateMap<InspectionPassFailItem, InspectionPassFailItemDTO>(MemberList.Destination);
    }
}