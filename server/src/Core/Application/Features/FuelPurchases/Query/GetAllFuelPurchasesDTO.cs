using System;

using Domain.Entities.Enums;

namespace Application.Features.FuelPurchases.Query.GetAllFuelPurchases;

public class GetAllFuelPurchasesDTO
{
    public int ID { get; set; }
    public required int VehicleId { get; set; }
    public required string PurchasedByUserId { get; set; }
    public required DateTime PurchaseDate { get; set; }
    public required double OdometerReading { get; set; }
    public required double Volume { get; set; }
    public required decimal PricePerUnit { get; set; }
    public required decimal TotalCost { get; set; }
    public required string FuelStation { get; set; }
    public required string ReceiptNumber { get; set; }
}