namespace Bazaar.Contracting.Domain.Abstractions;

public interface IUpdatePartnerEmailAddressService
{
    Result UpdatePartnerEmailAddress(
        string partnerExternalId, string emailAddress);
}
