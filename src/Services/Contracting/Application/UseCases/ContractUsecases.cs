namespace Bazaar.Contracting.Application;

public class ContractUsecases : IContractUsecases
{
    private readonly IContractRepository _contractRepo;
    private readonly IPartnerRepository _partnerRepo;
    private readonly ISellingPlanRepository _sellPlanRepo;

    public ContractUsecases(
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
        if (endDate.Date <= DateTime.Now.Date)
            return EndDateInvalid(
                nameof(endDate), "End date must be after current date.");

        var partner = _partnerRepo.GetWithContractsByExternalId(partnerExternalId);
        if (partner is null)
            return PartnerNotFound;

        if (partner.IsUnderContract)
            return PartnerUnderContract;

        var sellPlan = _sellPlanRepo.GetById(sellingPlanId);
        if (sellPlan is null)
            return SellingPlanNotFound;

        var contract = new Contract(partner, sellPlan, DateTime.Now.Date, endDate);
        _contractRepo.Create(contract);
        return Result.Success(new ContractDto(contract));
    }

    public Result<ContractDto> SignPartnerIndefinitely(
        string partnerExternalId, int sellingPlanId)
    {
        var partner = _partnerRepo.GetWithContractsByExternalId(partnerExternalId);
        if (partner is null)
            return PartnerNotFound;

        if (partner.IsUnderContract)
            return PartnerUnderContract;

        var sellPlan = _sellPlanRepo.GetById(sellingPlanId);
        if (sellPlan is null)
            return SellingPlanNotFound;

        var contract = new Contract(partner, sellPlan, DateTime.Now.Date, null);
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

        if (currentContract.EndDate is not null)
            return CurrentContractNotIndefinite;

        currentContract.UpdateEndDate(DateTime.Now.Date);
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

        if (currentContract.EndDate is null)
            return CurrentContractNotFixedPeriod;

        if (extendedEndDate <= currentContract.EndDate)
            return EndDateInvalid(
                nameof(extendedEndDate), "Extended end date must be after old end date.");

        currentContract.UpdateEndDate(extendedEndDate);
        _contractRepo.Update(currentContract);
        return Result.Success();
    }

    //
    // Helpers
    //
    private static Result PartnerNotFound
        => Result.NotFound($"Partner not found.");

    private static Result PartnerUnderContract
        => Result.Conflict("Partner is already under contract.");

    private static Result SellingPlanNotFound
        => Result.NotFound($"Selling plan not found.");

    private static Result CurrentContractNotFound
        => Result.NotFound($"Partner has no current contract.");

    private static Result CurrentContractNotIndefinite
        => Result.Conflict($"Current contract of partner is not indefinite.");

    private static Result CurrentContractNotFixedPeriod
        => Result.Conflict($"Current contract of partner is not fixed period.");

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
}
