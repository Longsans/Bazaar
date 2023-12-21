namespace Bazaar.Transport.Domain.Services;

public class BasicEstimationService : IEstimationService
{
    private readonly TimeSpan _perDeliveryItemIncrement = TimeSpan.FromMinutes(4);  // 1 hour = 15 items
    private readonly TimeSpan _perPickupItemIncrement = TimeSpan.FromMinutes(1.2);  // 1 hour = 50 items
    private readonly TimeSpan _perReturnUnitIncrement = TimeSpan.FromMinutes(2); // 1 hour = 30 items

    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IInventoryPickupRepository _pickupRepository;
    private readonly IInventoryReturnRepository _returnRepo;

    public BasicEstimationService(
        IDeliveryRepository deliveryRepository,
        IInventoryPickupRepository pickupRepository,
        IInventoryReturnRepository returnRepository)
    {
        _deliveryRepository = deliveryRepository;
        _pickupRepository = pickupRepository;
        _returnRepo = returnRepository;
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

    public DateTime EstimateInventoryReturnCompletion(IEnumerable<ReturnQuantity> returnQuantities)
    {
        var numOfIncompleteReturns = _returnRepo.GetIncomplete()
            .Sum(x => x.ReturnQuantities.Sum(item => item.Quantity));
        var currentLoadDelay = numOfIncompleteReturns * _perReturnUnitIncrement;
        var totalReturnUnits = returnQuantities.Sum(item => item.Quantity);
        return DateTime.Now + currentLoadDelay + totalReturnUnits * _perPickupItemIncrement;
    }
}
