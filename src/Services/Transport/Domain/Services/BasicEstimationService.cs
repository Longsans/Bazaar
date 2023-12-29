namespace Bazaar.Transport.Domain.Services;

public class BasicEstimationService : IEstimationService
{
    private readonly TimeSpan _perDeliveryItemIncrement;
    private readonly TimeSpan _perPickupItemIncrement;
    private readonly TimeSpan _perReturnUnitIncrement;

    private readonly IRepository<Delivery> _deliveryRepository;
    private readonly IRepository<InventoryPickup> _pickupRepository;
    private readonly IRepository<InventoryReturn> _returnRepo;

    public BasicEstimationService(
        IRepository<Delivery> deliveryRepository,
        IRepository<InventoryPickup> pickupRepository,
        IRepository<InventoryReturn> returnRepository,
        IConfiguration config)
    {
        _deliveryRepository = deliveryRepository;
        _pickupRepository = pickupRepository;
        _returnRepo = returnRepository;
        _perDeliveryItemIncrement = TimeSpan.FromMinutes(double.Parse(config["TimePerDeliveryItem"]!));
        _perPickupItemIncrement = TimeSpan.FromMinutes(double.Parse(config["TimePerPickupItem"]!));
        _perReturnUnitIncrement = TimeSpan.FromMinutes(double.Parse(config["TimePerReturnUnit"]!));
    }

    public async Task<DateTime> EstimateDeliveryCompletion(IEnumerable<DeliveryPackageItem> packageItems)
    {
        var incompleteDeliveries = await _deliveryRepository
            .ListAsync(new DeliveriesIncompleteSpec());

        return EstimateCompletion(
            packageItems,
            incompleteDeliveries,
            i => i.Quantity,
            d => d.PackageItems.Sum(item => item.Quantity));
    }

    public async Task<DateTime> EstimatePickupCompletion(IEnumerable<PickupProductStock> pickupItems)
    {
        var incompletePickups = await _pickupRepository
            .ListAsync(new PickupsIncompleteSpec());

        return EstimateCompletion(
            pickupItems,
            incompletePickups,
            i => i.NumberOfUnits,
            p => p.ProductStocks.Sum(item => item.NumberOfUnits));
    }

    public async Task<DateTime> EstimateInventoryReturnCompletion(IEnumerable<ReturnQuantity> returnQuantities)
    {
        var incompleteReturns = await _returnRepo
            .ListAsync(new ReturnsIncompleteSpec());

        return EstimateCompletion(
            returnQuantities,
            incompleteReturns,
            i => i.Quantity,
            r => r.ReturnQuantities.Sum(item => item.Quantity));
    }

    private DateTime EstimateCompletion<TEstimation, TExisting>(
        IEnumerable<TEstimation> estimationItems,
        IEnumerable<TExisting> existingItems,
        Func<TEstimation, long> estimationUnitsProjection,
        Func<TExisting, long> existingUnitsProjection)
    {
        var perUnitIncrement = existingItems switch
        {
            IEnumerable<Delivery> => _perDeliveryItemIncrement,
            IEnumerable<InventoryPickup> => _perPickupItemIncrement,
            IEnumerable<InventoryReturn> => _perReturnUnitIncrement,
            _ => throw new ArgumentException(
                "Type of existing transport items is not a valid estimation type.")
        };
        var currentIncompleteItemsUnits = existingItems.Sum(existingUnitsProjection);
        var unitsBeingEstimated = estimationItems.Sum(estimationUnitsProjection);
        var currentLoadDelay = currentIncompleteItemsUnits * perUnitIncrement;
        return DateTime.Now + currentLoadDelay + unitsBeingEstimated * perUnitIncrement;
    }
}
