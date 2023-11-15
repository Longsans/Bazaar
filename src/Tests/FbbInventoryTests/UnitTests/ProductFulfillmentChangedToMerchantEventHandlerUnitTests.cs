namespace FbbInventoryTests.UnitTests;

public class ProductFulfillmentChangedToMerchantEventHandlerUnitTests
{
    private readonly Mock<IProductInventoryRepository> _mockInventoryRepo;
    private readonly ProductFulfillmentChangedToMerchantIntegrationEventHandler _handler;

    private static readonly ProductInventory _testInventory = new("PROD-1", 100, 10, 1000, 1);

    public ProductFulfillmentChangedToMerchantEventHandlerUnitTests()
    {
        _mockInventoryRepo = new();
        _handler = new(_mockInventoryRepo.Object, new EventBusTestDouble());
    }

    [Fact]
    public async Task Handle_MarksInventoryAsUnfulfillable_WhenFound()
    {
        _mockInventoryRepo.Setup(x => x.GetByProductId(It.IsAny<string>()))
            .Returns(_testInventory);

        await _handler.Handle(new(_testInventory.ProductId));

        Assert.Equal(InventoryStatus.Unfulfillable, _testInventory.Status);
    }
}
