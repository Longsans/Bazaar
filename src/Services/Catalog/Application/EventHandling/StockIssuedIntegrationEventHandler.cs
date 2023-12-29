namespace Bazaar.Catalog.Application.EventHandling;

public class StockIssuedIntegrationEventHandler
    : IIntegrationEventHandler<StockIssuedIntegrationEvent>
{
    private readonly IRepository<CatalogItem> _catalogRepo;
    private readonly ILogger<StockIssuedIntegrationEventHandler> _logger;

    public StockIssuedIntegrationEventHandler(IRepository<CatalogItem> catalogRepo,
        ILogger<StockIssuedIntegrationEventHandler> logger)
    {
        _catalogRepo = catalogRepo;
        _logger = logger;
    }

    public async Task Handle(StockIssuedIntegrationEvent @event)
    {
        if (@event.IssueReason == StockIssueReason.Return)
        {
            if (@event.Items != null)
            {
                _logger.LogInformation("Notified of stock issued for return for products: {ProductIds}.",
                    string.Join(", ", @event.Items.Select(x => x.ProductId)));
            }
            return;
        }
        if (!Enum.IsDefined(@event.IssueReason))
        {
            _logger.LogError("Error handling stock issued integration event {EventId}: " +
                "StockIssueReason {IssueReason} is not defined.", @event.Id, @event.IssueReason);
        }

        var updatedItems = new List<CatalogItem>();
        foreach (var issueItem in @event.Items)
        {
            var catalogItem = await _catalogRepo.SingleOrDefaultAsync(
                new CatalogItemByProductIdSpec(issueItem.ProductId));
            if (catalogItem == null || catalogItem.IsDeleted)
            {
                _logger.LogInformation("Notified of stock issued for deleted listing {ProductId}",
                    issueItem.ProductId);
                continue;
            }

            try
            {
                catalogItem.ReduceStock(issueItem.GoodQuantity);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error reducing stock from issue for catalog item {ProductId}: {Ex}",
                    catalogItem.ProductId, ex.Message);
                return;
            }
            updatedItems.Add(catalogItem);
        }
        await _catalogRepo.UpdateRangeAsync(updatedItems);
        await Task.CompletedTask;
    }
}
