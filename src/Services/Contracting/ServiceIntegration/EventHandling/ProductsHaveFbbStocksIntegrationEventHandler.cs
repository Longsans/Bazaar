namespace Bazaar.Contracting.ServiceIntegration.EventHandling;

public class ProductsHaveFbbStocksIntegrationEventHandler
    : IIntegrationEventHandler<ProductsHaveFbbStocksIntegrationEvent>
{
    private readonly IClientRepository _clientRepo;

    public ProductsHaveFbbStocksIntegrationEventHandler(
        IClientRepository clientRepo)
    {
        _clientRepo = clientRepo;
    }

    public async Task Handle(ProductsHaveFbbStocksIntegrationEvent @event)
    {
        var client = _clientRepo.GetWithContractsAndPlanByExternalId(@event.SellerId);
        if (client != null && client.IsAccountClosed)
        {
            client.ReopenAccount();
            _clientRepo.Update(client);

            // Do other things like logging, notify clients,...
        }
        await Task.CompletedTask;
    }
}
