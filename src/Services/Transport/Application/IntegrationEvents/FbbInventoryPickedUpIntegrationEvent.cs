﻿namespace Bazaar.Transport.ServiceIntegration.IntegrationEvents;

public record FbbInventoryPickedUpIntegrationEvent(
    IEnumerable<PickupProductInventory> Inventories, string SchedulerId) : IntegrationEvent;

public record PickupProductInventory(string ProductId, uint StockUnits);