namespace Bazaar.Contracting.ServiceIntegration.EventHandling;

public class ProductsHaveFbbStocksIntegrationEventHandler
    : IIntegrationEventHandler<ProductsHaveFbbStocksIntegrationEvent>
{
    private readonly IRepository<Client> _clientRepo;

    public ProductsHaveFbbStocksIntegrationEventHandler(
        IRepository<Client> clientRepo)
    {
        _clientRepo = clientRepo;
    }

    public async Task Handle(ProductsHaveFbbStocksIntegrationEvent @event)
    {
        var client = await _clientRepo.SingleOrDefaultAsync(
            new ClientByExternalIdSpec(@event.SellerId, true));
        if (client != null && client.IsAccountClosed)
        {
            client.ReopenAccount();
            await _clientRepo.UpdateAsync(client);

            // Do other things like logging, notify clients,...
        }
        await Task.CompletedTask;
    }
}
