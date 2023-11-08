namespace Bazaar.Transport.Domain.Interfaces;

public interface ICompleteInventoryPickupService
{
    Result CompleteInventoryPickup(int id);
}
