namespace Bazaar.Contracting.Infrastructure.Repositories;

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
            .Include(c => c.Client)
            .Include(c => c.SellingPlan)
            .SingleOrDefault(c => c.Id == id);
    }

    public IEnumerable<Contract> GetByClientExternalId(string clientId)
    {
        return _context.Contracts
            .Include(c => c.Client)
            .Include(c => c.SellingPlan)
            .Where(c => c.Client.ExternalId == clientId);
    }

    public Contract Create(Contract contract)
    {
        _context.Contracts.Add(contract);
        _context.SaveChanges();
        return contract;
    }

    public void Update(Contract contract)
    {
        _context.Contracts.Update(contract);
        _context.SaveChanges();
    }
}
