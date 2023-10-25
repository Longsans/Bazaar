namespace Bazaar.Contracting.Domain.Interfaces;

public interface IUpdateClientEmailAddressService
{
    Result UpdateClientEmailAddress(
        string clientExternalId, string emailAddress);
}
