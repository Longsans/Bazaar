namespace Bazaar.Contracting.Domain.Abstractions;

public interface IPartnerRepository
{
    Partner? GetWithContractsById(int id);
    Partner? GetWithContractsByExternalId(string externalId);
    Partner? GetWithContractsByEmail(string email);
    Partner Create(Partner partner);
    void Update(Partner update);
    bool Delete(int id);
}