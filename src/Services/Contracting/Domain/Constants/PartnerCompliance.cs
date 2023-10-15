namespace Bazaar.Contracting.Domain.Constants;

public static class PartnerCompliance
{
    public const int MinimumAge = 18;

    public static readonly string PartnerMinimumAgeStatement
        = $"Partner must be {MinimumAge} or above.";

    public const string UniqueEmailAddressNonComplianceMessage
        = "Email address already used by another partner.";
}
