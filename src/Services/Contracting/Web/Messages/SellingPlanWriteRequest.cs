namespace Bazaar.Contracting.Web.Messages;

public record SellingPlanWriteRequest
{
    public string Name { get; set; }
    public decimal MonthlyFee { get; set; }
    public decimal PerSaleFee { get; set; }
    public float RegularPerSaleFeePercent { get; set; }
}
