namespace Bazaar.Contracting.Domain.Interfaces;

public interface IContractRepository
{
    Contract? GetById(int id);
    IEnumerable<Contract> GetByPartnerExternalId(string partnerId);
    Contract Create(Contract contract);
    void Update(Contract contract);
}