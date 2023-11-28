namespace Bazaar.Transport.Domain.Interfaces;

public interface IEstimationService
{
    DateTime EstimateDeliveryCompletion(IEnumerable<DeliveryPackageItem> packageItems);
    DateTime EstimatePickupCompletion(IEnumerable<ProductInventory> pickupItems);
    DateTime EstimateInventoryReturnCompletion(IEnumerable<ReturnQuantity> returnQuantities);
}
