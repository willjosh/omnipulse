using System;

using Application.Features.Issues.Command.CreateIssue;
using Application.Features.Issues.Command.UpdateIssue;

using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

public class IssueMappingProfile : Profile
{
    public IssueMappingProfile()
    {
        CreateMap<CreateIssueCommand, Issue>()
            .ForMember(dest => dest.ID, opt => opt.Ignore()) // Don't map ID - will be auto-generated
            .ForMember(dest => dest.IssueNumber, opt => opt.Ignore()) // Will be auto-generated
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.ResolvedDate, opt => opt.Ignore()) // Not set during creation
            .ForMember(dest => dest.ResolvedByUserID, opt => opt.Ignore()) // Not set during creation
            .ForMember(dest => dest.ResolutionNotes, opt => opt.Ignore()) // Not set during creation
            .ForMember(dest => dest.IssueAttachments, opt => opt.Ignore()) // Navigation collection
            .ForMember(dest => dest.IssueAssignments, opt => opt.Ignore()) // Navigation collection
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore()) // Navigation property
            .ForMember(dest => dest.ReportedByUser, opt => opt.Ignore()); // Navigation property

        CreateMap<UpdateIssueCommand, Issue>()
            .ForMember(dest => dest.ID, opt => opt.Ignore()) // Don't map ID - should not be changed
            .ForMember(dest => dest.IssueNumber, opt => opt.Ignore()) // Should not be changed
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.ResolvedDate, opt => opt.Ignore()) // Not set here
            .ForMember(dest => dest.ResolvedByUserID, opt => opt.Ignore()) // Not set here
            .ForMember(dest => dest.ResolutionNotes, opt => opt.Ignore()) // Not set here
            .ForMember(dest => dest.IssueAttachments, opt => opt.Ignore()) // Navigation collection
            .ForMember(dest => dest.IssueAssignments, opt => opt.Ignore()) // Navigation collection
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore()) // Navigation property
            .ForMember(dest => dest.ReportedByUser, opt => opt.Ignore()); // Navigation property

        CreateMap<Issue, Application.Features.Issues.Query.GetAllIssue.GetAllIssueDTO>()
            .ForMember(dest => dest.ReportedByUserName, opt => opt.MapFrom(src => src.ReportedByUser != null ? $"{src.ReportedByUser.FirstName} {src.ReportedByUser.LastName}" : ""))
            .ForMember(dest => dest.ResolvedByUserName, opt => opt.MapFrom(src => src.ResolvedByUser != null ? $"{src.ResolvedByUser.FirstName} {src.ResolvedByUser.LastName}" : null))
            .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Name : ""));

        CreateMap<Issue, Application.Features.Issues.Query.GetIssueDetails.GetIssueDetailsDTO>()
            .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Name : ""))
            .ForMember(dest => dest.ReportedByUserName, opt => opt.MapFrom(src => src.ReportedByUser != null ? $"{src.ReportedByUser.FirstName} {src.ReportedByUser.LastName}" : ""));
    }
}