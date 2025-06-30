using System;
using Domain.Entities.Enums;
using MediatR;

namespace Application.Features.Vehicles.Command.CreateVehicle;

public record CreateVehicleCommand(
   string Name,
   string Make,
   string Model,
   int Year,
   string VIN,
   string LicensePlate,
   DateTime LicensePlateExpirationDate,
   VehicleTypeEnum VehicleType,
   int VehicleGroupID,
   string Trim,
   double Mileage,
   double EngineHours,
   double FuelCapacity,
   FuelTypeEnum FuelType,
   DateTime PurchaseDate,
   decimal PurchasePrice,
   VehicleStatusEnum Status,
   string Location,
   string? AssignedTechnicianID
) : IRequest<int>;
