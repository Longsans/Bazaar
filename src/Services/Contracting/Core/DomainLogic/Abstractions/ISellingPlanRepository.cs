namespace Bazaar.Contracting.Core.DomainLogic;

public interface ISellingPlanRepository
{
    SellingPlan? GetById(int id);
    SellingPlan Create(SellingPlan plan);
    SellingPlan? Update(SellingPlan updatedPlan);
    void Delete(int id);
}
