namespace Bazaar.Contracting.Core.DomainLogic;

public interface IPartnerRepository
{
    Partner? GetWithContractsById(int id);
    Partner? GetWithContractsByExternalId(string externalId);
    Partner? GetWithContractsByEmail(string email);
    Partner Create(Partner partner);
    Partner? UpdateInfoByExternalId(Partner update);
    bool Delete(int id);
}