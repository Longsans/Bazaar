namespace Bazaar.Contracting.Core.DomainLogic;

public interface ISellingPlanManager
{
    SellingPlan? GetById(int id);
    ICreateSellingPlanResult CreateSellingPlan(SellingPlan plan);
    IUpdateSellingPlanResult UpdateSellingPlan(SellingPlan update);

}
