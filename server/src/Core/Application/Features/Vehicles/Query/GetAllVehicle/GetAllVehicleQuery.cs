using System;

using Application.Models;
using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.Vehicles.Query.GetAllVehicle;

public record GetAllVehicleQuery(PaginationParameters Parameters) : IRequest<PagedResult<GetAllVehicleDTO>> { }