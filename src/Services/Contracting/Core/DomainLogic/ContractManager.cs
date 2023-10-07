namespace Bazaar.Contracting.Core.DomainLogic;

public class ContractManager : IContractManager
{
    private readonly IContractRepository _contractRepo;
    private readonly IPartnerRepository _partnerRepo;
    private readonly ISellingPlanRepository _sellPlanRepo;

    public ContractManager(
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

    public ISignFixedPeriodContractResult SignPartnerForFixedPeriod(
        string partnerExternalId, int sellingPlanId, DateTime endDate)
    {
        var contract = new Contract()
        {
            StartDate = DateTime.Now.Date,
            EndDate = endDate.Date
        };

        if (!contract.HasValidEndDate)
            return ISignFixedPeriodContractResult.ContractEndDateBeforeCurrentDate;

        var partner = _partnerRepo.GetWithContractsByExternalId(partnerExternalId);
        if (partner is null)
            return ISignFixedPeriodContractResult.PartnerNotFoundError;

        if (partner.IsUnderContract)
            return ISignFixedPeriodContractResult
                .PartnerUnderContractError(partner.CurrentContract!);

        var sellPlan = _sellPlanRepo.GetById(sellingPlanId);
        if (sellPlan is null)
            return ISignFixedPeriodContractResult.SellingPlanNotFoundError;

        contract.Partner = partner;
        contract.SellingPlan = sellPlan;
        _contractRepo.Create(contract);
        return ISignFixedPeriodContractResult.Success(contract);
    }

    public ISignIndefiniteContractResult SignPartnerIndefinitely(
        string partnerExternalId, int sellingPlanId)
    {
        var partner = _partnerRepo.GetWithContractsByExternalId(partnerExternalId);
        if (partner is null)
            return ISignIndefiniteContractResult.PartnerNotFoundError;

        if (partner.IsUnderContract)
            return ISignIndefiniteContractResult
                .PartnerUnderContractError(partner.CurrentContract!);

        var sellPlan = _sellPlanRepo.GetById(sellingPlanId);
        if (sellPlan is null)
            return ISignIndefiniteContractResult.SellingPlanNotFoundError;

        var contract = new Contract()
        {
            Partner = partner,
            SellingPlan = sellPlan,
            StartDate = DateTime.Now.Date,
        };

        _contractRepo.Create(contract);
        return ISignIndefiniteContractResult.Success(contract);
    }

    public IEndContractResult EndCurrentIndefiniteContractWithPartner(
        string partnerExternalId)
    {
        var partner = _partnerRepo.GetWithContractsByExternalId(partnerExternalId);
        if (partner is null)
            return IEndContractResult.PartnerNotFoundError;

        var currentContract = partner.CurrentContract;
        if (currentContract is null)
            return IEndContractResult.ContractNotFoundError;

        if (currentContract.EndDate is not null)
            return IEndContractResult.ContractNotIndefiniteError;

        _contractRepo.UpdateEndDate(currentContract.Id, DateTime.Now.Date);
        return IEndContractResult.Success(currentContract);
    }

    public IExtendContractResult ExtendCurrentFixedPeriodContractWithPartner(
        string partnerExternalId, DateTime extendedEndDate)
    {
        extendedEndDate = extendedEndDate.Date;

        var partner = _partnerRepo.GetWithContractsByExternalId(partnerExternalId);
        if (partner is null)
            return IExtendContractResult.PartnerNotFoundError;

        var currentContract = partner.CurrentContract;
        if (currentContract is null)
            return IExtendContractResult.ContractNotFoundError;

        if (currentContract.EndDate is null)
            return IExtendContractResult.ContractNotFixedPeriodError;

        if (extendedEndDate <= currentContract.EndDate)
            return IExtendContractResult.EndDateNotAfterOldEndDateError;

        _contractRepo.UpdateEndDate(currentContract.Id, extendedEndDate);
        return IExtendContractResult.Success(currentContract);
    }
}
