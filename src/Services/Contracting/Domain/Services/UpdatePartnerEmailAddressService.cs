namespace Bazaar.Contracting.Domain.Services;

public class UpdatePartnerEmailAddressService : IUpdatePartnerEmailAddressService
{
    private readonly IPartnerRepository _partnerRepo;

    public UpdatePartnerEmailAddressService(IPartnerRepository partnerRepo)
    {
        _partnerRepo = partnerRepo;
    }

    public Result UpdatePartnerEmailAddress(string partnerExternalId, string emailAddress)
    {
        var partner = _partnerRepo.GetWithContractsByExternalId(partnerExternalId);
        if (partner == null)
        {
            return Result.NotFound("Partner not found");
        }

        var emailAddressOwner = _partnerRepo.GetWithContractsByEmailAddress(emailAddress);
        if (emailAddressOwner != null)
        {
            if (emailAddressOwner.Id != partner.Id)
            {
                return Result.Conflict(PartnerCompliance
                    .UniqueEmailAddressNonComplianceMessage);
            }
            return Result.Success();
        }

        partner.ChangeEmailAddress(emailAddress);
        _partnerRepo.Update(partner);
        return Result.Success();
    }
}
