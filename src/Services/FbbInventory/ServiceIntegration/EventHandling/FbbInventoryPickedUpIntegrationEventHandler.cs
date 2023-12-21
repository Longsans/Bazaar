namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

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
        var inboundQuantities = @event.Inventories.Select(x =>
            new InboundStockQuantity(x.ProductId, x.StockUnits));
        var result = _stockTxnService.ReceiveStock(@event.SchedulerId, inboundQuantities);
        if (!result.IsSuccess)
        {
            _logger.LogError("Error receiving stock from pickup for scheduler {SchedulerId}: {ErrorMessage}",
                @event.SchedulerId, result.GetJoinedErrorMessage());
        }

        await Task.CompletedTask;
    }
}
