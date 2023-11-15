namespace Bazaar.Transport.Domain.Services;

public class BasicEstimationService : IEstimationService
{
    private readonly TimeSpan _perDeliveryItemIncrement = TimeSpan.FromMinutes(4);  // 1 hour = 15 items
    private readonly TimeSpan _perPickupItemIncrement = TimeSpan.FromMinutes(1.2);  // 1 hour = 50 items

    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IInventoryPickupRepository _pickupRepository;

    public BasicEstimationService(
        IDeliveryRepository deliveryRepository,
        IInventoryPickupRepository pickupRepository)
    {
        _deliveryRepository = deliveryRepository;
        _pickupRepository = pickupRepository;
    }

    public DateTime EstimateDeliveryCompletion(IEnumerable<DeliveryPackageItem> packageItems)
    {
        var numOfIncompleteDeliveryItems = _deliveryRepository.GetIncomplete()
            .Sum(x => x.PackageItems.Sum(item => item.Quantity));
        var currentLoadDelay = numOfIncompleteDeliveryItems * _perDeliveryItemIncrement;
        var totalDeliveryQuantity = packageItems.Sum(item => item.Quantity);
        return DateTime.Now + currentLoadDelay + totalDeliveryQuantity * _perDeliveryItemIncrement;
    }

    public DateTime EstimatePickupCompletion(IEnumerable<ProductInventory> pickupItems)
    {
        var numOfIncompletePickupUnits = _pickupRepository.GetIncomplete()
            .Sum(x => x.ProductInventories.Sum(item => item.NumberOfUnits));
        var currentLoadDelay = numOfIncompletePickupUnits * _perPickupItemIncrement;
        var totalPickupUnits = pickupItems.Sum(item => item.NumberOfUnits);
        return DateTime.Now + currentLoadDelay + totalPickupUnits * _perPickupItemIncrement;
    }
}
