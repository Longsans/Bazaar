namespace Bazaar.Contracting.Application;

public class PartnerUseCases : IPartnerUseCases
{
    private readonly IPartnerRepository _partnerRepo;
    private readonly IUpdatePartnerEmailAddressService _updateEmailAddressService;

    public PartnerUseCases(
        IPartnerRepository partnerRepo,
        IUpdatePartnerEmailAddressService updateEmailService)
    {
        _partnerRepo = partnerRepo;
        _updateEmailAddressService = updateEmailService;
    }

    public Partner? GetWithContractsById(int partnerId)
    {
        return _partnerRepo.GetWithContractsById(partnerId);
    }

    public Partner? GetWithContractsByExternalId(string partnerExternalId)
    {
        return _partnerRepo.GetWithContractsByExternalId(partnerExternalId);
    }

    public Result<PartnerDto> RegisterPartner(PartnerDto partnerDto)
    {
        Partner partner;
        try
        {
            partner = partnerDto.ToNewPartner();
        }
        catch (PartnerUnderMinimumAgeException)
        {
            return PartnerUnderMinimumAge(nameof(PartnerDto.DateOfBirth));
        }

        var existingEmailAddressOwner = _partnerRepo
            .GetWithContractsByEmailAddress(partnerDto.Email);

        if (existingEmailAddressOwner is not null)
        {
            return Result.Conflict(
                PartnerCompliance.UniqueEmailAddressNonComplianceMessage);
        }

        _partnerRepo.Create(partner);
        return Result.Success(new PartnerDto(partner));
    }

    public Result UpdatePartnerInfoByExternalId(PartnerDto partnerDto)
    {
        var partner = _partnerRepo
            .GetWithContractsByExternalId(partnerDto.ExternalId!);
        if (partner == null)
        {
            return Result.NotFound();
        }

        Partner updatedPartner;
        partnerDto.Id = partner.Id;
        try
        {
            updatedPartner = partnerDto.ToExistingPartner();
        }
        catch (PartnerUnderMinimumAgeException)
        {
            return PartnerUnderMinimumAge(nameof(PartnerDto.DateOfBirth));
        }

        var existingEmailOwner = _partnerRepo
            .GetWithContractsByEmailAddress(partnerDto.Email);

        if (existingEmailOwner != null
            && existingEmailOwner.ExternalId != partner.ExternalId)
        {
            return Result.Conflict(
                PartnerCompliance.UniqueEmailAddressNonComplianceMessage);
        }

        _partnerRepo.Update(updatedPartner);
        return Result.Success();
    }

    public Result UpdatePartnerEmailAddress(string partnerExternalId, string newEmail)
    {
        return _updateEmailAddressService
            .UpdatePartnerEmailAddress(partnerExternalId, newEmail);
    }

    private static Result PartnerUnderMinimumAge(string dobPropName)
    {
        return Result.Invalid(new()
        {
            new()
            {
                Identifier = dobPropName,
                ErrorMessage = PartnerCompliance.PartnerMinimumAgeStatement
            }
        });
    }
}
