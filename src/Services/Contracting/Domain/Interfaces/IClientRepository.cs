namespace Bazaar.Contracting.Domain.Interfaces;

public interface IClientRepository
{
    Client? GetWithContractsById(int id);
    Client? GetWithContractsByExternalId(string externalId);
    Client? GetWithContractsByEmailAddress(string email);
    Client Create(Client client);
    void Update(Client update);
    bool Delete(int id);
}