namespace Bazaar.Contracting.Core.DomainLogic;

public interface ISellingPlanRepository
{
    SellingPlan? GetById(int id);
    SellingPlan Create(SellingPlan plan);
    SellingPlan? FindAndUpdate(SellingPlan updatedPlan);
    void Delete(int id);
}
