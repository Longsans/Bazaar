using Bazaar.Contracting.Application.IntegrationEvents;

namespace Bazaar.Contracting.Application.EventHandling;

public class ProductListingsDeleteFailedIntegrationEventHandler
    : IIntegrationEventHandler<ProductListingsDeleteFailedIntegrationEvent>
{
    private readonly IRepository<Client> _clientRepo;
    private readonly ILogger<ProductListingsDeleteFailedIntegrationEventHandler> _logger;

    public ProductListingsDeleteFailedIntegrationEventHandler(
        IRepository<Client> clientRepo,
        ILogger<ProductListingsDeleteFailedIntegrationEventHandler> logger)
    {
        _clientRepo = clientRepo;
        _logger = logger;
    }

    public async Task Handle(ProductListingsDeleteFailedIntegrationEvent @event)
    {
        _logger.LogCritical("Product listings delete failed for the following reasons: {reasons}",
            string.Join(", ", @event.FailedListings.Select(x => $"Product {x.ProductId} {x.FailureReason}")));

        var client = await _clientRepo.SingleOrDefaultAsync(
            new ClientByExternalIdSpec(@event.SellerId, true));
        if (client != null && client.IsAccountClosed)
        {
            client.ReopenAccount();
            await _clientRepo.UpdateAsync(client);

            _logger.LogCritical("Client account {clientId} has been reopened after listings delete failure", client.ExternalId);
            // Notify clients via email, messages, etc.
            //
        }
        await Task.CompletedTask;
    }
}
