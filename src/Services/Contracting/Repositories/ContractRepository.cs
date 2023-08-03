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

    public void CreateFixedPeriod(Contract contract)
    {
        if (!contract.IsInsertable)
        {
            return;
        }

        var partner = _context.Partners
            .Include(p => p.Contracts)
            .FirstOrDefault(p => p.Id == contract.PartnerId);
        if (partner == null)
        {
            return;
        }

        if (partner.IsUnderContract)
        {
            return;
        }

        partner.Contracts.Add(contract);
        _context.SaveChanges();
    }

    public void CreateIndefinite(Contract contract)
    {
        if (!contract.IsInsertable)
        {
            Console.WriteLine("Contract not insertable");
            return;
        }

        var partner = _context.Partners
            .Include(p => p.Contracts)
            .FirstOrDefault(p => p.Id == contract.PartnerId);
        if (partner == null)
        {
            Console.WriteLine("Partner null");
            return;
        }

        if (partner.IsUnderContract)
        {
            Console.WriteLine("Partner under contract");
            return;
        }

        contract.EndDate = null;
        partner.Contracts.Add(contract);
        _context.SaveChanges();
    }

    public void EndContract(int id)
    {
        var contract = _context.Contracts.FirstOrDefault(c => c.Id == id);
        if (contract == null)
        {
            return;
        }

        if (contract.EndDate != null)
        {
            return;
        }

        contract.EndDate = DateTime.Now;
        _context.SaveChanges();
    }
}
