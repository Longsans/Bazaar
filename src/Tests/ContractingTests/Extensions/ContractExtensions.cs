namespace ContractingTests.Extensions;

public static class ContractExtensions
{
    private const int INVALID_PARTNER_ID = 0;
    private const int INVALID_PLAN_ID = 0;
    private static readonly DateTime PAST_END_DATE = DateTime.Now.Date - TimeSpan.FromDays(1);
    private const int CONTRACTED_PARTNER_ID = 2;

    public static Contract Clone(this Contract contract)
        => new()
        {
            Id = contract.Id,
            Partner = contract.Partner,
            PartnerId = contract.PartnerId,
            SellingPlan = contract.SellingPlan,
            SellingPlanId = contract.SellingPlanId,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
        };

    public static Contract WithInvalidPartner(this Contract contract)
    {
        var clone = contract.Clone();
        clone.PartnerId = INVALID_PARTNER_ID;
        return clone;
    }

    public static Contract WithInvalidSellingPlan(this Contract contract)
    {
        var clone = contract.Clone();
        clone.SellingPlanId = INVALID_PLAN_ID;
        return clone;
    }

    public static Contract WithPastEndDate(this Contract contract)
    {
        var clone = contract.Clone();
        clone.EndDate = PAST_END_DATE;
        return clone;
    }

    public static Contract WithAlreadyContractedPartner(this Contract contract)
    {
        var clone = contract.Clone();
        clone.PartnerId = CONTRACTED_PARTNER_ID;
        return clone;
    }
}
