namespace Bazaar.Contracting.Domain.Interfaces;

public interface IUpdatePartnerEmailAddressService
{
    Result UpdatePartnerEmailAddress(
        string partnerExternalId, string emailAddress);
}
