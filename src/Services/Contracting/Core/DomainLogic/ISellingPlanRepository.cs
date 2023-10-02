namespace Bazaar.Contracting.Core.DomainLogic;

public interface ISellingPlanRepository
{
    SellingPlan? GetById(int id);
}
