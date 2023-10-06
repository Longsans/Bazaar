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

    public SellingPlan Create(SellingPlan plan)
    {
        _context.SellingPlans.Add(plan);
        _context.SaveChanges();
        return plan;
    }

    public SellingPlan? Update(SellingPlan updatedPlan)
    {
        var existing = _context.SellingPlans.Find(updatedPlan.Id);

        if (existing != null)
        {
            existing.Name = updatedPlan.Name;
            existing.PerSaleFee = updatedPlan.PerSaleFee;
            existing.MonthlyFee = updatedPlan.MonthlyFee;
            existing.RegularPerSaleFeePercent = updatedPlan.RegularPerSaleFeePercent;
            _context.SaveChanges();
        }

        return existing;
    }

    public void Delete(int id)
    {
        var existing = _context.SellingPlans.Find(id);
        if (existing != null)
            _context.SellingPlans.Remove(existing);
    }
}
