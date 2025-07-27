using MediatR;

namespace Application.Features.FuelLogging.Command.CreateFuelPurchase;

public record CreateFuelPurchaseCommand(
    int VehicleId,
    string PurchasedByUserId,
    DateTime PurchaseDate,
    double OdometerReading,
    double Volume,
    decimal PricePerUnit,
    decimal TotalCost,
    string FuelStation,
    string ReceiptNumber,
    string? Notes
) : IRequest<int>;