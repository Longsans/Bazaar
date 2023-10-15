namespace Bazaar.Contracting.Domain.Exceptions;

public class RegularPerSaleFeePercentNotPositiveException
    : Exception
{
    public RegularPerSaleFeePercentNotPositiveException()
        : base("Regular per sale fee percentage must be greater than 0 for selling plan.") { }
}
