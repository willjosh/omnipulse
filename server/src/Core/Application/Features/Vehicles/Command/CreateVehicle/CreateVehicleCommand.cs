using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.Vehicles.Command.CreateVehicle;

/// <summary>
/// Command for creating a new vehicle.
/// </summary>
/// <param name="Name">The display name for the vehicle.</param>
/// <param name="Make">The manufacturer of the vehicle (e.g., "Ford", "Toyota").</param>
/// <param name="Model">The specific model of the vehicle (e.g., "F-150", "Camry").</param>
/// <param name="Year">The manufacturing year of the vehicle.</param>
/// <param name="VIN">The Vehicle Identification Number - must be unique.</param>
/// <param name="LicensePlate">The vehicle's license plate number - must be unique.</param>
/// <param name="LicensePlateExpirationDate">The expiration date of the license plate registration.</param>
/// <param name="VehicleType">The type/category of vehicle. See <see cref="VehicleTypeEnum"/> for options.</param>
/// <param name="VehicleGroupID">The ID of the vehicle group this vehicle belongs to.</param>
/// <param name="Trim">The specific trim level or configuration of the vehicle.</param>
/// <param name="Mileage">The current odometer reading in miles or kilometres.</param>
/// <param name="EngineHours">The total engine operating hours (for applicable vehicles).</param>
/// <param name="FuelCapacity">The fuel tank capacity in litres.</param>
/// <param name="FuelType">The type of fuel the vehicle uses. See <see cref="FuelTypeEnum"/> for options.</param>
/// <param name="PurchaseDate">The date when the vehicle was acquired.</param>
/// <param name="PurchasePrice">The purchase price of the vehicle.</param>
/// <param name="Status">The current operational status of the vehicle. See <see cref="VehicleStatusEnum"/> for options.</param>
/// <param name="Location">The current location of the vehicle.</param>
/// <param name="AssignedTechnicianID">Optional ID of the technician responsible for this vehicle.</param>
/// <returns>The ID of the newly created vehicle.</returns>
/// <remarks>
/// This command implements the Command pattern using MediatR for creating vehicles.
/// It validates business rules such as unique VIN and license plate constraints,
/// and ensures the assigned technician and vehicle group exist before creation.
///
/// Example usage:
/// <code language="csharp">
/// var command = new CreateVehicleCommand(
///     Name: "Fleet Vehicle 001",
///     Make: "Ford",
///     Model: "Transit",
///     Year: 2024,
///     VIN: "1FTBW2CM0PKA12345",
///     LicensePlate: "ABC123",
///     LicensePlateExpirationDate: DateTime.Now.AddYears(1),
///     VehicleType: VehicleTypeEnum.Van,
///     VehicleGroupID: 1,
///     Trim: "Base",
///     Mileage: 0,
///     EngineHours: 0,
///     FuelCapacity: 70.0,
///     FuelType: FuelTypeEnum.Diesel,
///     PurchaseDate: DateTime.Now,
///     PurchasePrice: 45000m,
///     Status: VehicleStatusEnum.Active,
///     Location: "Main Depot",
///     AssignedTechnicianID: null
/// );
/// </code>
/// </remarks>
/// <exception cref="Application.Exceptions.BadRequestException">
/// Thrown when validation fails for any of the input parameters.
/// </exception>
/// <exception cref="Application.Exceptions.DuplicateEntityException">
/// Thrown when a vehicle with the same VIN or license plate already exists.
/// </exception>
/// <exception cref="Application.Exceptions.EntityNotFoundException">
/// Thrown when the specified VehicleGroupID or AssignedTechnicianID does not exist.
/// </exception>
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