﻿namespace Bazaar.FbbInventory.Application.EventHandling;

public class FbbInventoryPickedUpIntegrationEventHandler
    : IIntegrationEventHandler<FbbInventoryPickedUpIntegrationEvent>
{
    private readonly StockTransactionService _stockTxnService;
    private readonly ILogger<FbbInventoryPickedUpIntegrationEventHandler> _logger;

    public FbbInventoryPickedUpIntegrationEventHandler(StockTransactionService stockTxnService,
        ILogger<FbbInventoryPickedUpIntegrationEventHandler> logger)
    {
        _stockTxnService = stockTxnService;
        _logger = logger;
    }

    public async Task Handle(FbbInventoryPickedUpIntegrationEvent @event)
    {
        var productWithNoStock = @event.Inventories.FirstOrDefault(x => x.StockUnits == 0);
        if (productWithNoStock != null)
        {
            _logger.LogError("Error receiving stock from pickup for scheduler {schedulerId}: " +
                "Product {productId} of pickup has zero stock units.",
                @event.SchedulerId, productWithNoStock.ProductId);
        }

        var inboundQuantities = @event.Inventories.Select(x =>
            new InboundStockQuantity(x.ProductId, x.StockUnits));
        var result = await _stockTxnService.ReceiveStock(inboundQuantities);
        if (!result.IsSuccess)
        {
            _logger.LogError("Error receiving stock from pickup for scheduler {SchedulerId}: {ErrorMessage}",
                @event.SchedulerId, result.GetJoinedErrorMessage());
        }

        await Task.CompletedTask;
    }
}
