using MediatR;

namespace Application.Features.FuelPurchases.Command.UpdateFuelPurchase;

public record UpdateFuelPurchaseCommand(
    int FuelPurchaseId,
    int VehicleId,
    string PurchasedByUserId,
    DateTime PurchaseDate,
    double OdometerReading,
    double Volume,
    decimal PricePerUnit,
    string FuelStation,
    string ReceiptNumber,
    string? Notes
) : IRequest<int>;