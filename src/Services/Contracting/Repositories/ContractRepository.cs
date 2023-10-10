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
            .SingleOrDefault(c => c.Id == id);
    }

    public IEnumerable<Contract> GetByPartnerExternalId(string partnerId)
    {
        return _context.Contracts
            .Include(c => c.Partner)
            .Include(c => c.SellingPlan)
            .Where(c => c.Partner.ExternalId == partnerId);
    }

    public Contract Create(Contract contract)
    {
        _context.Contracts.Add(contract);
        _context.SaveChanges();
        return contract;
    }

    public Contract? FindAndUpdateEndDate(int id, DateTime endDate)
    {
        var contract = _context.Contracts.Find(id);
        if (contract is not null && endDate >= DateTime.Now.Date) // safeguard end date so that no ridiculous accident happens
        {
            contract.EndDate = endDate;
            _context.SaveChanges();
            return contract;
        }
        else
        {
            return null;
        }
    }
}
