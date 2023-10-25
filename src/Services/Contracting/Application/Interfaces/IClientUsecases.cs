namespace Bazaar.Contracting.Application;

public interface IClientUseCases
{
    Client? GetWithContractsById(int clientId);
    Client? GetWithContractsByExternalId(string clientExternalId);
    Result<ClientDto> RegisterClient(ClientDto client);
    Result UpdateClientInfoByExternalId(ClientDto client);
    Result UpdateClientEmailAddress(string clientExternalId, string newEmail);
}
