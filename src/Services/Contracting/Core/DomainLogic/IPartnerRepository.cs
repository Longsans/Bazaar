namespace Bazaar.Contracting.Core.DomainLogic;

public interface IPartnerRepository
{
    Partner? GetWithContractsById(int id);
    Partner? GetWithContractsByExternalId(string externalId);
    Partner Create(Partner partner);
    bool UpdateInfo(Partner update);
    bool Delete(int id);
}