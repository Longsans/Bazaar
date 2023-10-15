namespace Bazaar.Contracting.Domain.Exceptions;

public class MonthlyAndPerSaleFeesNotPositiveException
    : Exception
{
    public MonthlyAndPerSaleFeesNotPositiveException()
        : base(PlanRequirements.MonthlyFeeOrPerSaleFeeGreaterThanZeroStatement) { }
}
