namespace Bazaar.Contracting.Application;

public class PartnerUsecases : IPartnerUsecases
{
    private readonly IPartnerRepository _partnerRepo;
    private readonly IUpdatePartnerEmailAddressService _updateEmailAddressService;

    public PartnerUsecases(
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
        if (DateTime.Now.Year - partnerDto.DateOfBirth.Year < PartnerCompliance.MinimumAge)
        {
            return Result.Invalid(new()
            {
                new()
                {
                    Identifier = nameof(partnerDto.DateOfBirth),
                    ErrorMessage = PartnerCompliance.PartnerMinimumAgeStatement
                }
            });
        }

        var possibleExisting = _partnerRepo.GetWithContractsByEmail(partnerDto.Email);
        if (possibleExisting is not null)
        {
            return Result.Conflict(
                PartnerCompliance.UniqueEmailAddressNonComplianceMessage);
        }

        var partner = partnerDto.ToNewPartner();
        _partnerRepo.Create(partner);
        return Result.Success(new PartnerDto(partner));
    }

    public Result UpdatePartnerInfoByExternalId(PartnerDto partnerDto)
    {
        if (DateTime.Now.Year - partnerDto.DateOfBirth.Year < PartnerCompliance.MinimumAge)
        {
            return Result.Invalid(new()
            {
                new()
                {
                    Identifier = nameof(partnerDto.DateOfBirth),
                    ErrorMessage = PartnerCompliance.PartnerMinimumAgeStatement
                }
            });
        }

        var partner = _partnerRepo.GetWithContractsByExternalId(partnerDto.ExternalId!);
        if (partner == null)
        {
            return Result.NotFound();
        }

        var existingEmailOwner = _partnerRepo.GetWithContractsByEmail(partnerDto.Email);

        if (existingEmailOwner != null
            && existingEmailOwner.ExternalId != partner.ExternalId)
        {
            return Result.Conflict(
                PartnerCompliance.UniqueEmailAddressNonComplianceMessage);
        }

        partnerDto.Id = partner.Id;
        _partnerRepo.Update(partnerDto.ToExistingPartner());
        return Result.Success();
    }

    public Result UpdatePartnerEmailAddress(string partnerExternalId, string newEmail)
    {
        return _updateEmailAddressService
            .UpdatePartnerEmailAddress(partnerExternalId, newEmail);
    }
}
