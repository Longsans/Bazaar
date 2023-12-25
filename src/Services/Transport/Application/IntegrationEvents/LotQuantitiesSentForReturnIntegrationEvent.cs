namespace Bazaar.Transport.ServiceIntegration.IntegrationEvents;

public record LotQuantitiesSentForReturnIntegrationEvent(
    IEnumerable<LotQuantity> LotQuantities,
    string DeliveryAddress, string InventoryOwnerId) : IntegrationEvent;

public record LotQuantity(
    string LotNumber, uint Quantity);