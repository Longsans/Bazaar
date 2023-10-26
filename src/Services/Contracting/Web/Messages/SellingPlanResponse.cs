namespace Bazaar.Contracting.Web.Messages;

public record SellingPlanResponse
{
    public int Id { get; private set; }
    public string Name { get; set; }
    public decimal MonthlyFee { get; private set; }
    public decimal PerSaleFee { get; private set; }
    public float RegularPerSaleFeePercent { get; private set; }

    public SellingPlanResponse(SellingPlan plan)
    {
        Id = plan.Id;
        Name = plan.Name;
        MonthlyFee = plan.MonthlyFee;
        PerSaleFee = plan.PerSaleFee;
        RegularPerSaleFeePercent = plan.RegularPerSaleFeePercent;
    }
}
