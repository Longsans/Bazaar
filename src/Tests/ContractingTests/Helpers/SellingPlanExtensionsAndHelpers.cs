namespace ContractingTests.Helpers;

public static class SellingPlanExtensionsAndHelpers
{
    public static readonly SellingPlan ExistingPlan = new()
    {
        Id = 1,
        Name = "Individual",
        MonthlyFee = 0m,
        PerSaleFee = 0.99m,
        RegularPerSaleFeePercent = 0.08f
    };

    public static readonly SellingPlan ValidNewPlan = new()
    {
        Name = "Business",
        MonthlyFee = 39.99m,
        RegularPerSaleFeePercent = 0.1f
    };

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
