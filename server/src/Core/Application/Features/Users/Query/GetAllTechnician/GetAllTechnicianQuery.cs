using System;

using Application.Models;
using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.Users.Query.GetAllTechnician;

public record GetAllTechnicianQuery(PaginationParameters Parameters) : IRequest<PagedResult<GetAllTechnicianDTO>> { }