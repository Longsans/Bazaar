namespace Bazaar.Contracting.Application;

public interface ISellingPlanUseCases
{
    SellingPlan? GetById(int id);
    Result<SellingPlanDto> CreateSellingPlan(SellingPlanDto planDto);
    Result UpdateSellingPlan(SellingPlanDto updateDto);
}
