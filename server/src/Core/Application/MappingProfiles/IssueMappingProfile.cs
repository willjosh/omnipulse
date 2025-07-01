using System;
using Application.Features.Issues.Command.CreateIssue;
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
            .ForMember(dest => dest.ResolvedBy, opt => opt.Ignore()) // Not set during creation
            .ForMember(dest => dest.ResolutionNotes, opt => opt.Ignore()) // Not set during creation
            .ForMember(dest => dest.IssueAttachments, opt => opt.Ignore()) // Navigation collection
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore()) // Navigation property
            .ForMember(dest => dest.User, opt => opt.Ignore()); // Navigation property
    }
}