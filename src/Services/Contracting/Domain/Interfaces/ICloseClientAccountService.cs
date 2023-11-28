namespace Bazaar.Contracting.Domain.Interfaces;

public interface ICloseClientAccountService
{
    Result CloseAccount(string clientExternalId);
}
