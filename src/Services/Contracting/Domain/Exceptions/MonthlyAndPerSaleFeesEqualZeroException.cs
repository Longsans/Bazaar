namespace Bazaar.Contracting.Domain.Exceptions;

public class MonthlyAndPerSaleFeesEqualZeroException
    : Exception
{
    public MonthlyAndPerSaleFeesEqualZeroException()
        : base(PlanRequirements.MonthlyFeeOrPerSaleFeeGreaterThanZeroStatement) { }
}
