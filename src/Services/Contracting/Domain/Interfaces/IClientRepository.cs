namespace Bazaar.Contracting.Domain.Interfaces;

public interface IClientRepository
{
    Client? GetWithContractsAndPlanById(int id);
    Client? GetWithContractsAndPlanByExternalId(string externalId);
    Client? GetWithContractsAndPlanByEmailAddress(string emailAddress);
    Client Create(Client client);
    void Update(Client update);
    bool Delete(int id);
}