namespace Bazaar.Transport.Domain.Services;

public class BasicEstimationService : IEstimationService
{
    private readonly TimeSpan _perDeliveryItemIncrement;
    private readonly TimeSpan _perPickupItemIncrement;
    private readonly TimeSpan _perReturnUnitIncrement;

    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IInventoryPickupRepository _pickupRepository;
    private readonly IInventoryReturnRepository _returnRepo;

    public BasicEstimationService(
        IDeliveryRepository deliveryRepository,
        IInventoryPickupRepository pickupRepository,
        IInventoryReturnRepository returnRepository,
        IConfiguration config)
    {
        _deliveryRepository = deliveryRepository;
        _pickupRepository = pickupRepository;
        _returnRepo = returnRepository;
        _perDeliveryItemIncrement = TimeSpan.FromMinutes(double.Parse(config["TimePerDeliveryItem"]!));
        _perPickupItemIncrement = TimeSpan.FromMinutes(double.Parse(config["TimePerPickupItem"]!));
        _perReturnUnitIncrement = TimeSpan.FromMinutes(double.Parse(config["TimePerReturnUnit"]!));
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
