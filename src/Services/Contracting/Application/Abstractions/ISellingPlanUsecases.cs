namespace Bazaar.Contracting.Application;

public interface ISellingPlanUsecases
{
    SellingPlan? GetById(int id);
    Result<SellingPlanDto> CreateSellingPlan(SellingPlanDto planDto);
    Result UpdateSellingPlan(SellingPlanDto updateDto);
}
