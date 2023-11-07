namespace Bazaar.Contracting.Domain.Interfaces;

public interface IContractRepository
{
    Contract? GetById(int id);
    IEnumerable<Contract> GetByClientExternalId(string clientId);
    Contract Create(Contract contract);
    void Update(Contract contract);
}