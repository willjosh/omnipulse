using System;
using Domain.Entities.Enums;
using MediatR;

namespace Application.Features.Vehicles.Command.UpdateVehicle;

public record UpdateVehicleCommand(
   int VehicleID,
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
   double PurchasePrice,
   VehicleStatusEnum Status,
   string Location
) : IRequest<int>;
