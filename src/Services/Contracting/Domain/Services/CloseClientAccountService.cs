namespace Bazaar.Contracting.Domain.Services;

public class CloseClientAccountService : ICloseClientAccountService
{
    private readonly IClientRepository _clientRepo;
    private readonly IEventBus _eventBus;

    public CloseClientAccountService(
        IClientRepository clientRepo, IEventBus eventBus)
    {
        _clientRepo = clientRepo;
        _eventBus = eventBus;
    }

    public Result CloseAccount(string clientExternalId)
    {
        var client = _clientRepo
            .GetWithContractsAndPlanByExternalId(clientExternalId);
        if (client == null || client.IsAccountClosed)
        {
            return Result.NotFound("Client not found.");
        }

        client.CloseAccount();
        _clientRepo.Update(client);
        _eventBus.Publish(
            new ClientAccountClosedIntegrationEvent(client.ExternalId));

        return Result.Success();
    }
}
