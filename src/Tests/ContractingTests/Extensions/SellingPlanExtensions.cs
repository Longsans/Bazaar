namespace ContractingTests.Extensions;

public static class SellingPlanExtensions
{
    public static SellingPlan Clone(this SellingPlan plan)
    {
        return new()
        {
            Id = plan.Id,
            Name = plan.Name,
            MonthlyFee = plan.MonthlyFee,
            PerSaleFee = plan.PerSaleFee,
            RegularPerSaleFeePercent = plan.RegularPerSaleFeePercent,
        };
    }
}
