namespace Bazaar.Contracting.Application.Services;

public class CloseClientAccountService
{
    private readonly Repository<Client> _clientRepo;
    private readonly IEventBus _eventBus;

    public CloseClientAccountService(
        Repository<Client> clientRepo, IEventBus eventBus)
    {
        _clientRepo = clientRepo;
        _eventBus = eventBus;
    }

    public async Task<Result> CloseAccount(string clientExternalId)
    {
        var client = await _clientRepo.SingleOrDefaultAsync(
            new ClientByExternalIdSpec(clientExternalId));
        if (client == null)
        {
            return Result.NotFound("Client not found.");
        }

        client.CloseAccount();
        await _clientRepo.UpdateAsync(client);
        _eventBus.Publish(
            new ClientAccountClosedIntegrationEvent(client.ExternalId));

        return Result.Success();
    }
}
