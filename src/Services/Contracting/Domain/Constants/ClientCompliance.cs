namespace Bazaar.Contracting.Domain.Constants;

public static class ClientCompliance
{
    public const int MinimumAge = 18;

    public static readonly string ClientMinimumAgeStatement
        = $"Client must be {MinimumAge} or above.";

    public const string UniqueEmailAddressNonComplianceMessage
        = "Email address already used by another client.";
}
