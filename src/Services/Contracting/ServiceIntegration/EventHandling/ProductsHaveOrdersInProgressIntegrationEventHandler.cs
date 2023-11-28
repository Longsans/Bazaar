namespace Bazaar.Contracting.ServiceIntegration.EventHandling;

public class ProductsHaveOrdersInProgressIntegrationEventHandler
    : IIntegrationEventHandler<ProductsHaveOrdersInProgressIntegrationEvent>
{
    private readonly IClientRepository _clientRepo;

    public ProductsHaveOrdersInProgressIntegrationEventHandler(
        IClientRepository clientRepo)
    {
        _clientRepo = clientRepo;
    }

    public async Task Handle(ProductsHaveOrdersInProgressIntegrationEvent @event)
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
