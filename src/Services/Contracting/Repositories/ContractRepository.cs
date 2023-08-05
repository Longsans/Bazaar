namespace Bazaar.Contracting.Repositories;

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

    public ICreateFixedPeriodResult CreateFixedPeriod(Contract contract)
    {
        if (!contract.IsInsertable)
        {
            return ICreateFixedPeriodResult.ContractStartDateInPastOrAfterEndDateError;
        }

        var partner = _context.Partners
            .Include(p => p.Contracts)
            .FirstOrDefault(p => p.Id == contract.PartnerId);
        if (partner == null)
        {
            return ICreateFixedPeriodResult.PartnerNotFoundError;
        }

        if (partner.IsUnderContract)
        {
            return ICreateFixedPeriodResult.PartnerUnderContractError;
        }

        partner.Contracts.Add(contract);
        _context.SaveChanges();
        return ICreateFixedPeriodResult.Success;
    }

    public ICreateIndefiniteResult CreateIndefinite(Contract contract)
    {
        if (!contract.IsInsertable)
        {
            return ICreateIndefiniteResult.ContractStartDateInPastOrAfterEndDateError;
        }

        var partner = _context.Partners
            .Include(p => p.Contracts)
            .FirstOrDefault(p => p.Id == contract.PartnerId);
        if (partner == null)
        {
            return ICreateIndefiniteResult.PartnerNotFoundError;
        }

        if (partner.IsUnderContract)
        {
            return ICreateIndefiniteResult.PartnerUnderContractError;
        }

        contract.EndDate = null;
        partner.Contracts.Add(contract);
        _context.SaveChanges();

        return ICreateIndefiniteResult.Success;
    }

    public IEndContractResult EndIndefiniteContract(int partnerId)
    {
        var partner = _context.Partners
                            .Include(p => p.Contracts)
                            .FirstOrDefault(p => p.Id == partnerId);
        if (partner == null)
        {
            return IEndContractResult.PartnerNotFoundError;
        }

        var currentContract = partner.CurrentContract;
        if (currentContract == null)
        {
            return IEndContractResult.ContractNotFoundError;
        }

        if (currentContract.EndDate != null)
        {
            return IEndContractResult.ContractNotIndefiniteError;
        }

        currentContract.EndDate = DateTime.Now;
        _context.SaveChanges();
        return IEndContractResult.Success(currentContract);
    }
}
