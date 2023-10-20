namespace Bazaar.Contracting.Domain.Exceptions;

public class PartnerUnderMinimumAgeException : Exception
{
    public PartnerUnderMinimumAgeException(int minimumAge)
        : base($"Partner must be {minimumAge} and above.") { }
}
