namespace Bazaar.Contracting.DTOs;

public record SellingPlanWriteCommand
{
    public string Name { get; set; }
    public decimal MonthlyFee { get; set; }
    public decimal PerSaleFee { get; set; }
    public float RegularPerSaleFeePercent { get; set; }

    public SellingPlan ToSellingPlan(int id) => new()
    {
        Id = id,
        Name = Name,
        MonthlyFee = MonthlyFee,
        PerSaleFee = PerSaleFee,
        RegularPerSaleFeePercent = RegularPerSaleFeePercent
    };

    public SellingPlan PlanInfo() => new()
    {
        Name = Name,
        MonthlyFee = MonthlyFee,
        PerSaleFee = PerSaleFee,
        RegularPerSaleFeePercent = RegularPerSaleFeePercent
    };
}
