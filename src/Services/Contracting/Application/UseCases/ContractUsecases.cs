namespace Bazaar.Contracting.Application;

public class ContractUseCases : IContractUseCases
{
    private readonly IContractRepository _contractRepo;
    private readonly IPartnerRepository _partnerRepo;
    private readonly ISellingPlanRepository _sellPlanRepo;

    public ContractUseCases(
        IContractRepository contractRepo,
        IPartnerRepository partnerRepo,
        ISellingPlanRepository sellPlanRepo)
    {
        _contractRepo = contractRepo;
        _partnerRepo = partnerRepo;
        _sellPlanRepo = sellPlanRepo;
    }

    public Contract? GetById(int id)
    {
        return _contractRepo.GetById(id);
    }

    public IEnumerable<Contract> GetByPartnerExternalId(string partnerExternalId)
    {
        return _contractRepo.GetByPartnerExternalId(partnerExternalId);
    }

    public Result<ContractDto> SignPartnerForFixedPeriod(
        string partnerExternalId, int sellingPlanId, DateTime endDate)
    {
        var partner = _partnerRepo.GetWithContractsByExternalId(partnerExternalId);
        if (partner is null)
            return PartnerNotFound;

        if (partner.IsUnderContract)
            return PartnerUnderContract;

        var sellingPlan = _sellPlanRepo.GetById(sellingPlanId);
        if (sellingPlan is null)
            return SellingPlanNotFound;

        try
        {
            var contract = new Contract(partner.Id, sellingPlan.Id, endDate);
            _contractRepo.Create(contract);
            return Result.Success(new ContractDto(contract));
        }
        catch (EndDateBeforeCurrentDateException)
        {
            return EndDateInvalid(
                nameof(endDate), "End date cannot be before current date.");
        }
    }

    public Result<ContractDto> SignPartnerIndefinitely(
        string partnerExternalId, int sellingPlanId)
    {
        var partner = _partnerRepo.GetWithContractsByExternalId(partnerExternalId);
        if (partner is null)
            return PartnerNotFound;

        if (partner.IsUnderContract)
            return PartnerUnderContract;

        var sellingPlan = _sellPlanRepo.GetById(sellingPlanId);
        if (sellingPlan is null)
            return SellingPlanNotFound;

        var contract = new Contract(partner.Id, sellingPlan.Id, null);
        _contractRepo.Create(contract);
        return Result.Success(new ContractDto(contract));
    }

    public Result EndCurrentIndefiniteContractWithPartner(
        string partnerExternalId)
    {
        var partner = _partnerRepo.GetWithContractsByExternalId(partnerExternalId);
        if (partner is null)
            return PartnerNotFound;

        var currentContract = partner.CurrentContract;
        if (currentContract is null)
            return CurrentContractNotFound;

        try
        {
            currentContract.End();
        }
        catch (ContractEndedException)
        {
            return LatestContractEnded;
        }

        _contractRepo.Update(currentContract);
        return Result.Success();
    }

    public Result ExtendCurrentFixedPeriodContractWithPartner(
        string partnerExternalId, DateTime extendedEndDate)
    {
        extendedEndDate = extendedEndDate.Date;

        var partner = _partnerRepo.GetWithContractsByExternalId(partnerExternalId);
        if (partner is null)
            return PartnerNotFound;

        var currentContract = partner.CurrentContract;
        if (currentContract is null)
            return CurrentContractNotFound;

        try
        {
            currentContract.Extend(extendedEndDate);
        }
        catch (Exception ex)
        {
            return ex switch
            {
                ExtendIndefiniteContractException => CurrentContractNotFixedPeriod,
                ContractEndedException => LatestContractEnded,
                ExtendedEndDateNotAfterOriginalEndDateException
                 => EndDateInvalid(
                     nameof(extendedEndDate),
                     "Extended end date must be after old end date.")
            };
        }
        _contractRepo.Update(currentContract);
        return Result.Success();
    }

    #region Helpers
    private static Result PartnerNotFound
        => Result.NotFound($"Partner not found.");

    private static Result PartnerUnderContract
        => Result.Conflict("Partner is already under contract.");

    private static Result SellingPlanNotFound
        => Result.NotFound($"Selling plan not found.");

    private static Result CurrentContractNotFound
        => Result.NotFound($"Partner has no current contract.");

    private static Result CurrentContractNotFixedPeriod
        => Result.Conflict($"Current contract of partner is not fixed period.");

    private static Result LatestContractEnded
        => Result.Error($"Latest contract has ended.");

    private static Result EndDateInvalid(
        string endDatePropName, string message)
    {
        return Result.Invalid(new()
        {
            new()
            {
                Identifier = endDatePropName,
                ErrorMessage = message
            }
        });
    }
    #endregion
}
