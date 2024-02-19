namespace OrderingTests.UnitTests;

public class HandleOrderServiceUnitTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly EventBusTestDouble _testEventBus;
    private readonly HandleOrderService _service;
    private readonly Order _testOrder;

    public HandleOrderServiceUnitTests(EventBusTestDouble eventBus)
    {
        _testEventBus = eventBus;
        _orderRepoMock = new();
        _service = new(_orderRepoMock.Object, _testEventBus);
        _testOrder = new("1600 Pennsylvania, Washington D.C., United States", "SPER-1", new OrderItem[]
        {
            new("PROD-1", "The Winds of Winter", 39.99m, 2, 1),
            new("PROD-2", "A Dream of Spring", 29.99m, 1, 1),
        });
        _orderRepoMock.Setup(x => x.GetById(It.IsAny<int>()))
            .Returns(_testOrder);
        _orderRepoMock.Setup(x => x.GetContainsProduct(It.IsIn("PROD-1", "PROD-2")))
            .Returns(new List<Order>() { _testOrder });
    }

    [Fact]
    public void PlaceOrder_PublishesEventsAndReturnsOrder()
    {
        _service.PlaceOrder(_testOrder);

        AssertAllOrderedProductsHadEventsPublished(
            _testOrder.Items.Select(x => OrderStatusReport.FromInProgress(x.ProductId, 1)));
    }

    [Fact]
    public void UpdateOrderStatusToShipped_PublishesEventsAndReturnsSuccess_WhenValid()
    {
        // arrange
        typeof(Order).GetProperty(nameof(_testOrder.Status))
            .SetValue(_testOrder, GetValidStatusForTransitionTo(OrderStatus.Shipped));

        // act
        var result = _service.UpdateOrderStatus(1, OrderStatus.Shipped);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Shipped, _testOrder.Status);

        AssertAllOrderedProductsHadEventsPublished(
            _testOrder.Items.Select(x => OrderStatusReport.FromShipped(x.ProductId, 1)));
    }

    [Theory]
    [InlineData(OrderStatus.PendingSellerConfirmation)]
    [InlineData(OrderStatus.Postponed)]
    public void UpdateOrderStatusToCancelled_PublishesEventsAndReturnsSuccess_WhenValid(
        OrderStatus originalStatus)
    {
        typeof(Order).GetProperty(nameof(_testOrder.Status))
            .SetValue(_testOrder, originalStatus);
        var reason = "Flood ruined inventory.";

        var result = _service.UpdateOrderStatus(1, OrderStatus.Cancelled, reason);

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Cancelled, _testOrder.Status);
        Assert.Equal(reason, _testOrder.CancelReason);
        AssertAllOrderedProductsHadEventsPublished(
            _testOrder.Items.Select(x => OrderStatusReport.FromCancelled(x.ProductId, 1)));
    }

    #region Helpers
    private static OrderStatus GetValidStatusForTransitionTo(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.ProcessingPayment => OrderStatus.PendingValidation,
            OrderStatus.PendingSellerConfirmation => OrderStatus.ProcessingPayment,
            OrderStatus.Shipping => OrderStatus.PendingSellerConfirmation,
            OrderStatus.Shipped => OrderStatus.Shipping,
            _ => throw new InvalidOperationException()
        };
    }

    private void AssertAllOrderedProductsHadEventsPublished(
        IEnumerable<OrderStatusReport> expectedReports)
    {
        var publishedEvents = _testEventBus
                .GetEvents<ProductOrdersStatusReportChangedIntegrationEvent>();
        bool eventsWerePublishedForAllProducts = _testOrder.Items
            .Select(x => x.ProductId)
            .All(id => publishedEvents.Select(e => e.ProductId).Contains(id));

        Assert.True(eventsWerePublishedForAllProducts);
        foreach (var report in expectedReports)
        {
            var @event = publishedEvents.Single(e => e.ProductId == report.ProductId);
            Assert.Equal(report.InProgress, @event.OrdersInProgress);
            Assert.Equal(report.Shipped, @event.ShippedOrders);
            Assert.Equal(report.Cancelled, @event.CancelledOrders);
        }
    }

    class OrderStatusReport
    {
        public string ProductId { get; set; }
        public uint InProgress { get; set; }
        public uint Shipped { get; set; }
        public uint Cancelled { get; set; }

        public OrderStatusReport(string productId)
        {
            ProductId = productId;
        }

        public static OrderStatusReport FromInProgress(string productId, uint inProgress)
            => new(productId) { InProgress = inProgress };
        public static OrderStatusReport FromShipped(string productId, uint shipped)
            => new(productId) { Shipped = shipped };
        public static OrderStatusReport FromCancelled(string productId, uint cancelled)
            => new(productId) { Cancelled = cancelled };
    }
    #endregion
}
