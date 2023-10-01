namespace WebSellerUI.Model;

public class FixedPeriodContractCreateCommand
{
    public int SellingPlanId { get; set; }
    public DateTime EndDate { get; set; }
}

public record IndefiniteContractCreateCommand
{
    public int SellingPlanId { get; set; }
}

public record IndefiniteContractEndCommand(
    bool Ended
);