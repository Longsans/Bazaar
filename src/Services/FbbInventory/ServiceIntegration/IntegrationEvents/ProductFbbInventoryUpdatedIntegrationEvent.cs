﻿namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record ProductFbbInventoryUpdatedIntegrationEvent(
    string ProductId, uint FulfillableStock, uint UnfulfillableStock,
    uint FulfillableUnitsPendingRemoval,
    uint UnfulfillableUnitsPendingRemoval) : IntegrationEvent;
