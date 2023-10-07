namespace Bazaar.Contracting.Core.DomainLogic;

public interface IPartnerManager
{
    Partner? GetWithContractsById(int partnerId);
    Partner? GetWithContractsByExternalId(string partnerExternalId);
    IRegisterPartnerResult RegisterPartner(Partner partner);
    IUpdatePartnerInfoResult UpdatePartnerInfoByExternalId(Partner partner);
}
