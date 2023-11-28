namespace FbbInventoryTests.UnitTests;

public class ProductFulfillmentChangedToMerchantEventHandlerUnitTests
{
    private readonly Mock<IProductInventoryRepository> _mockInventoryRepo;
    private readonly ProductFulfillmentChangedToMerchantIntegrationEventHandler _handler;

    private static readonly ProductInventory _testInventory = new("PROD-1", 100, 5, 10, 10, 1000, 1);

    public ProductFulfillmentChangedToMerchantEventHandlerUnitTests()
    {
        _mockInventoryRepo = new();
        _handler = new(_mockInventoryRepo.Object, new EventBusTestDouble());
    }
}
