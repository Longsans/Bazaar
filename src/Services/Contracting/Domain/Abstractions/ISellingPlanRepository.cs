namespace Bazaar.Contracting.Domain.Abstractions;

public interface ISellingPlanRepository
{
    SellingPlan? GetById(int id);
    SellingPlan Create(SellingPlan plan);
    void Update(SellingPlan plan);
    void Delete(int id);
}
