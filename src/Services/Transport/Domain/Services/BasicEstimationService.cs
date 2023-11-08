namespace Bazaar.Transport.Domain.Services;

public class BasicEstimationService : IEstimationService
{
    private readonly TimeSpan _perDeliveryItemIncrement = TimeSpan.FromHours(1);
    private readonly TimeSpan _perPickupItemIncrement = TimeSpan.FromMinutes(30);

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
        var currentLoadDelay = EstimateDelayFromCurrentLoad();
        var totalDeliveryQuantity = packageItems.Sum(item => item.Quantity);
        return DateTime.Now + currentLoadDelay + totalDeliveryQuantity * _perDeliveryItemIncrement;
    }

    public DateTime EstimatePickupCompletion(IEnumerable<ProductInventory> pickupItems)
    {
        var currentLoadDelay = EstimateDelayFromCurrentLoad();
        var totalPickupUnits = pickupItems.Sum(item => item.NumberOfUnits);
        return DateTime.Now + currentLoadDelay + totalPickupUnits * _perPickupItemIncrement;
    }

    private TimeSpan EstimateDelayFromCurrentLoad()
    {
        var numOfIncompleteDeliveryItems = _deliveryRepository.GetIncomplete()
            .Sum(x => x.Items.Sum(item => item.Quantity));
        var numOfIncompletePickupUnits = _pickupRepository.GetIncomplete()
            .Sum(x => x.ProductInventories.Sum(item => item.NumberOfUnits));

        var baseDelayMultiplier = (numOfIncompleteDeliveryItems + numOfIncompletePickupUnits) / 100;
        return baseDelayMultiplier > 1
            ? TimeSpan.FromDays(baseDelayMultiplier)
            : TimeSpan.FromDays(1);
    }
}
