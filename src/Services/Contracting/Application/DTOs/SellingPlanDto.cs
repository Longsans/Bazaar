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

    public SellingPlan ToExistingPlan()
    {
        if (Id is null)
            throw new NullReferenceException("Id must not be null.");

        return new SellingPlan(
            Id.Value, Name, MonthlyFee,
            PerSaleFee, RegularPerSaleFeePercent);
    }
}
