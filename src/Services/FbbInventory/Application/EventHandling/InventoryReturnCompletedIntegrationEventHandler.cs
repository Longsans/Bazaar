﻿namespace Bazaar.FbbInventory.Application.EventHandling;

public class InventoryReturnCompletedIntegrationEventHandler
    : IIntegrationEventHandler<InventoryReturnCompletedIntegrationEvent>
{
    private readonly Repository<Lot> _lotRepo;
    private readonly Repository<ProductInventory> _productInventoryRepo;

    public InventoryReturnCompletedIntegrationEventHandler(
        Repository<Lot> lotRepo, Repository<ProductInventory> productInventoryRepo)
    {
        _lotRepo = lotRepo;
        _productInventoryRepo = productInventoryRepo;
    }

    public async Task Handle(InventoryReturnCompletedIntegrationEvent @event)
    {
        var returnedInventories = new List<ProductInventory>();
        foreach (var returnedQuantity in @event.ReturnedLotQuantities)
        {
            var lot = await _lotRepo.SingleOrDefaultAsync(
                new LotWithInventoriesSpec(returnedQuantity.LotNumber));
            if (lot == null)
            {
                // Should never be possible
                return;
            }

            lot.ConfirmUnitsRemoved(returnedQuantity.Quantity);
            var inventory = lot.ProductInventory;
            inventory.RemoveEmptyLots();
            returnedInventories.Add(inventory);
        }
        await _productInventoryRepo.UpdateRangeAsync(returnedInventories);

        await Task.CompletedTask;
    }
}
