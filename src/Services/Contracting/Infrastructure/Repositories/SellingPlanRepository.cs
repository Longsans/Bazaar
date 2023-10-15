namespace Bazaar.Contracting.Infrastructure.Repositories;

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

    public SellingPlan Create(SellingPlan plan)
    {
        _context.SellingPlans.Add(plan);
        _context.SaveChanges();
        return plan;
    }

    public void Update(SellingPlan plan)
    {
        _context.SellingPlans.Update(plan);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var existing = _context.SellingPlans.Find(id);
        if (existing != null)
            _context.SellingPlans.Remove(existing);
    }
}
