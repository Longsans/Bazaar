namespace Bazaar.Contracting.Repositories;

public class SellingPlanRepository : ISellingPlanRepository
{
    private readonly ContractingDbContext _context;

    public SellingPlanRepository(ContractingDbContext context)
    {
        _context = context;
    }

    public SellingPlan? GetById(int id)
    {
        return _context.SellingPlans.Find(id);
    }
}
