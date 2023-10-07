namespace Bazaar.Contracting.Core.DomainLogic;

public class SellingPlanManager : ISellingPlanManager
{
    private readonly ISellingPlanRepository _planRepo;

    public SellingPlanManager(ISellingPlanRepository planRepo)
    {
        _planRepo = planRepo;
    }

    public SellingPlan? GetById(int id)
    {
        return _planRepo.GetById(id);
    }

    public ICreateSellingPlanResult CreateSellingPlan(SellingPlan plan)
    {
        if (plan.PerSaleFee <= 0m && plan.MonthlyFee <= 0m)
            return ICreateSellingPlanResult.PerSaleFeeAndMonthlyFeeNotPositive;

        if (plan.RegularPerSaleFeePercent <= 0f)
            return ICreateSellingPlanResult.RegularPerSaleFeeNotPositive;

        var createdPlan = _planRepo.Create(plan);
        return ICreateSellingPlanResult.Success(createdPlan);
    }

    public IUpdateSellingPlanResult UpdateSellingPlan(SellingPlan update)
    {
        if (update.PerSaleFee <= 0m && update.MonthlyFee <= 0m)
            return IUpdateSellingPlanResult.PerSaleFeeAndMonthlyFeeNotPositive;

        if (update.RegularPerSaleFeePercent <= 0f)
            return IUpdateSellingPlanResult.RegularPerSaleFeeNotPositive;

        var updatedPlan = _planRepo.FindAndUpdate(update);
        if (updatedPlan == null)
            return IUpdateSellingPlanResult.SellingPlanNotFound;

        return IUpdateSellingPlanResult.Success(updatedPlan);
    }
}
