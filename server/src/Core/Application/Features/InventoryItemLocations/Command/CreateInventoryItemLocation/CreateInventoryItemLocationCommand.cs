using MediatR;
namespace Application.Features.InventoryItemLocations.Command;

public record CreateInventoryItemLocationCommand
(
    string LocationName,
    string Address,
    double Longitude,
    double Latitude,
    int Capacity
) : IRequest<int>;