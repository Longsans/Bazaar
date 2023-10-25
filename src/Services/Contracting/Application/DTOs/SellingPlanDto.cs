namespace Bazaar.Contracting.Application.DTOs;

public class SellingPlanDto
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public decimal MonthlyFee { get; set; }
    public decimal PerSaleFee { get; set; }
    public float RegularPerSaleFeePercent { get; set; }

    public SellingPlanDto() { }

    public SellingPlanDto(SellingPlan plan)
    {
        Id = plan.Id;
        Name = plan.Name;
        MonthlyFee = plan.MonthlyFee;
        PerSaleFee = plan.PerSaleFee;
        RegularPerSaleFeePercent = plan.RegularPerSaleFeePercent;
    }

    public SellingPlan ToNewPlan()
    {
        return new SellingPlan(
            Name, MonthlyFee,
            PerSaleFee, RegularPerSaleFeePercent);
    }
}
