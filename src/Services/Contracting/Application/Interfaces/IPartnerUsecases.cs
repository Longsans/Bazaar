namespace Bazaar.Contracting.Application;

public interface IPartnerUseCases
{
    Partner? GetWithContractsById(int partnerId);
    Partner? GetWithContractsByExternalId(string partnerExternalId);
    Result<PartnerDto> RegisterPartner(PartnerDto partner);
    Result UpdatePartnerInfoByExternalId(PartnerDto partner);
}
