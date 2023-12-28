namespace Bazaar.Transport.Domain.Interfaces;

public interface IEstimationService
{
    Task<DateTime> EstimateDeliveryCompletion(IEnumerable<DeliveryPackageItem> packageItems);
    Task<DateTime> EstimatePickupCompletion(IEnumerable<PickupProductStock> pickupItems);
    Task<DateTime> EstimateInventoryReturnCompletion(IEnumerable<ReturnQuantity> returnQuantities);
}
