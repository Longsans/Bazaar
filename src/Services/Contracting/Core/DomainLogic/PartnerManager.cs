namespace Bazaar.Contracting.Core.DomainLogic;

public class PartnerManager
{
    private readonly IPartnerRepository _partnerRepo;

    public PartnerManager(IPartnerRepository partnerRepo)
    {
        _partnerRepo = partnerRepo;
    }

    public Partner? GetWithContractsById(int partnerId)
    {
        return _partnerRepo.GetWithContractsById(partnerId);
    }

    public Partner? GetWithContractsByExternalId(string partnerExternalId)
    {
        return _partnerRepo.GetWithContractsByExternalId(partnerExternalId);
    }

    public IRegisterPartnerResult RegisterPartner(Partner partner)
    {
        if (DateTime.Now.Year - partner.DateOfBirth.Year < 18)
            return IRegisterPartnerResult.PartnerUnderEighteen;

        var possibleExisting = _partnerRepo.GetWithContractsByEmail(partner.Email);
        if (possibleExisting is not null)
            return IRegisterPartnerResult.PartnerEmailAlreadyExists;

        partner.Contracts = new List<Contract>();
        var registeredPartner = _partnerRepo.Create(partner);
        return IRegisterPartnerResult.Success(registeredPartner);
    }

    public IUpdatePartnerInfoResult UpdatePartnerInfoByExternalId(Partner partner)
    {
        if (DateTime.Now.Year - partner.DateOfBirth.Year < 18)
            return IUpdatePartnerInfoResult.PartnerUnderEighteen;

        var possibleExisting = _partnerRepo.GetWithContractsByEmail(partner.Email);

        if (possibleExisting != null
            && possibleExisting.ExternalId != partner.ExternalId)
            return IUpdatePartnerInfoResult.PartnerEmailAlreadyExists;

        var updatedPartner = _partnerRepo.UpdateInfoByExternalId(partner);
        if (updatedPartner == null)
            return IUpdatePartnerInfoResult.PartnerNotFound;

        return IUpdatePartnerInfoResult.Success(updatedPartner);
    }
}
