﻿namespace Bazaar.Contracting.Repositories;

public class ContractRepository : IContractRepository
{
    private readonly ContractingDbContext _context;

    public ContractRepository(ContractingDbContext context)
    {
        _context = context;
    }

    public Contract? GetById(int id)
    {
        return _context.Contracts
            .Include(c => c.Partner)
            .Include(c => c.SellingPlan)
            .FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<Contract> GetByPartnerId(string partnerId)
    {
        return _context.Contracts
            .Include(c => c.Partner)
            .Include(c => c.SellingPlan)
            .Where(c => c.Partner.ExternalId == partnerId);
    }

    public ICreateFixedPeriodResult CreateFixedPeriod(Contract contract)
    {
        contract.StartDate = DateTime.Now.Date;
        contract.EndDate = contract.EndDate?.Date;

        if (!contract.HasValidEndDate)
            return ICreateFixedPeriodResult.ContractEndDateBeforeCurrentDate;

        var partner = _context.Partners
            .Include(p => p.Contracts)
            .FirstOrDefault(p => p.Id == contract.PartnerId);

        if (partner == null)
            return ICreateFixedPeriodResult.PartnerNotFoundError;

        if (partner.IsUnderContract)
            return ICreateFixedPeriodResult.PartnerUnderContractError;

        var sellingPlan = _context.SellingPlans.Find(contract.SellingPlanId);
        if (sellingPlan is null)
            return ICreateFixedPeriodResult.SellingPlanNotFoundError;

        partner.Contracts.Add(contract);
        _context.SaveChanges();
        return ICreateFixedPeriodResult.Success(contract);
    }

    public ICreateIndefiniteResult CreateIndefinite(Contract contract)
    {
        contract.StartDate = DateTime.Now.Date;
        contract.EndDate = null;

        var partner = _context.Partners
            .Include(p => p.Contracts)
            .FirstOrDefault(p => p.Id == contract.PartnerId);

        if (partner == null)
            return ICreateIndefiniteResult.PartnerNotFoundError;

        if (partner.IsUnderContract)
            return ICreateIndefiniteResult.PartnerUnderContractError;

        var sellingPlan = _context.SellingPlans.Find(contract.SellingPlanId);
        if (sellingPlan is null)
            return ICreateIndefiniteResult.SellingPlanNotFoundError;

        partner.Contracts.Add(contract);
        _context.SaveChanges();
        return ICreateIndefiniteResult.Success(contract);
    }

    public IEndContractResult EndIndefiniteContract(int partnerId)
    {
        var partner = _context.Partners
                            .Include(p => p.Contracts)
                            .FirstOrDefault(p => p.Id == partnerId);

        if (partner == null)
            return IEndContractResult.PartnerNotFoundError;

        var currentContract = partner.CurrentContract;
        if (currentContract == null)
            return IEndContractResult.ContractNotFoundError;

        if (currentContract.EndDate != null)
            return IEndContractResult.ContractNotIndefiniteError;

        currentContract.EndDate = DateTime.Now.Date;
        _context.SaveChanges();
        return IEndContractResult.Success(currentContract);
    }

    public IExtendContractResult ExtendFixedPeriodContract(int id, DateTime extendedEndDate)
    {
        var contract = _context.Contracts.Find(id);
        extendedEndDate = extendedEndDate.Date;

        if (contract is null)
            return IExtendContractResult.ContractNotFoundError;

        if (contract.EndDate is null)
            return IExtendContractResult.ContractNotFixedPeriodError;

        if (contract.EndDate < DateTime.Now.Date)
            return IExtendContractResult.ContractEndedError;

        if (extendedEndDate <= contract.EndDate)
            return IExtendContractResult.EndDateNotAfterOldEndDateError;

        contract.EndDate = extendedEndDate.Date;
        _context.SaveChanges();
        return IExtendContractResult.Success(contract);
    }
}
