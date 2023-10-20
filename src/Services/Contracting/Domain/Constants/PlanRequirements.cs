namespace Bazaar.Contracting.Domain.Constants;

public static class PlanRequirements
{
    public const string MonthlyFeeOrPerSaleFeeGreaterThanZeroStatement
        = "Either monthly fee or per sale fee has to be greater than 0.";

    public const string RegularPerSaleFeePercentGreaterThanZeroStatement
        = "Regular per sale fee percentage has to be greater than 0.";
}
