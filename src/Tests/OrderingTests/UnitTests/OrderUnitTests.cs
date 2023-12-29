namespace OrderingTests.UnitTests;

public class OrderUnitTests
{
    #region Test data and helpers
    private const string _productId = "PROD-1";
    private const string _productName = "The Winds of Winter";
    private const decimal _validPrice = 39.99m;
    private const uint _validQuantity = 10;
    private const int _orderId = 1;

    private const string _validShippingAddress = "308 Negra Arroyo Lane, Albuquerque, New Mexico, U.S.";
    private const string _buyerId = "SPER-1";

    private const string _orderCancelReason = "Heavy rain flooded the warehouse and ruined most of the stocks.";

    public static IEnumerable<object[]> UncancellableStatuses
        => GetStatusValuesExcluding(OrderStatus.PendingSellerConfirmation, OrderStatus.Postponed);

    public static IEnumerable<object[]> UnpayableStatuses
        => GetStatusValuesExcluding(OrderStatus.PendingValidation);

    public static IEnumerable<object[]> SellerConfirmationUnawaitableStatuses
        => GetStatusValuesExcluding(OrderStatus.ProcessingPayment);

    public static IEnumerable<object[]> UnshippableStatuses
        => GetStatusValuesExcluding(OrderStatus.PendingSellerConfirmation);

    public static IEnumerable<object[]> ShippedUnconfirmableStatuses
        => GetStatusValuesExcluding(OrderStatus.Shipping);

    public static IEnumerable<object[]> UnpostponableStatuses
        => GetStatusValuesExcluding(OrderStatus.ProcessingPayment,
            OrderStatus.PendingSellerConfirmation, OrderStatus.Shipping);

    private static Order GetTestOrder()
    {
        var orderItem = new OrderItem(
            _productId, _productName, _validPrice, _validQuantity, default);
        var orderItem2 = new OrderItem(
            "PROD-2", "A Dream of Spring", _validPrice, _validQuantity, default);

        var order = new Order(_validShippingAddress, _buyerId, new OrderItem[]
        {
            orderItem,
            orderItem2,
        });

        return order;
    }

    private static void SetStatus(Order order, OrderStatus status)
    {
        typeof(Order).GetProperty(nameof(order.Status))!
            .SetValue(order, status);
    }

    private static IEnumerable<object[]> GetStatusValuesExcluding(
        params OrderStatus[] excludedValues)
    {
        return Enum.GetValues<OrderStatus>()
            .Where(s => !excludedValues.Contains(s))
            .Select(s => new object[] { s });
    }
    #endregion

    [Fact]
    public void OrderItemConstructor_Succeeds_WhenValid()
    {
        var orderItem = new OrderItem(
            _productId, _productName, _validPrice, _validQuantity, _orderId);

        Assert.Equal(OrderItemStatus.PendingStock, orderItem.Status);
        Assert.Equal(_validPrice, orderItem.ProductUnitPrice);
        Assert.Equal(_validQuantity, orderItem.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-39.99)]
    public void OrderItemConstructor_ThrowsArgOutOfRangeException_WhenProductPriceNotMoreThanZero(
        decimal price)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var orderItem = new OrderItem(
                _productId, _productName, price, _validQuantity, _orderId);
        });
    }

    [Fact]
    public void OrderItemConstructor_ThrowsArgOutOfRangeException_WhenQuantityIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var orderItem = new OrderItem(
                _productId, _productName, _validPrice, 0, _orderId);
        });
    }

    [Fact]
    public void OrderItem_SetStockConfirmed_Succeeds_WhenStatusIsPendingStock()
    {
        var item = GetTestOrder().Items.First();

        item.SetStockConfirmed();

        Assert.Equal(OrderItemStatus.StockConfirmed, item.Status);
    }

    [Theory]
    [InlineData(OrderItemStatus.StockConfirmed)]
    [InlineData(OrderItemStatus.StockRejected)]
    public void OrderItem_SetStockConfirmed_ThrowsInvalidOpException_WhenStatusNotPendingStock(
        OrderItemStatus status)
    {
        var item = GetTestOrder().Items.First();
        typeof(OrderItem).GetProperty(nameof(item.Status))!
            .SetValue(item, status);

        Assert.Throws<InvalidOperationException>(item.SetStockConfirmed);
        if (status != OrderItemStatus.StockConfirmed)
        {
            Assert.NotEqual(OrderItemStatus.StockConfirmed, item.Status);
        }
    }

    [Fact]
    public void OrderItem_SetStockRejected_Succeeds_WhenStatusIsPendingStock()
    {
        var item = GetTestOrder().Items.First();

        item.SetStockRejected();

        Assert.Equal(OrderItemStatus.StockRejected, item.Status);
    }

    [Theory]
    [InlineData(OrderItemStatus.StockConfirmed)]
    [InlineData(OrderItemStatus.StockRejected)]
    public void OrderItem_SetStockRejected_ThrowsInvalidOpException_WhenStatusNotPendingStock(
        OrderItemStatus status)
    {
        var item = GetTestOrder().Items.First();
        typeof(OrderItem).GetProperty(nameof(item.Status))!
            .SetValue(item, status);

        Assert.Throws<InvalidOperationException>(item.SetStockRejected);
        if (status != OrderItemStatus.StockConfirmed)
        {
            Assert.NotEqual(OrderItemStatus.StockConfirmed, item.Status);
        }
    }

    [Fact]
    public void OrderConstructor_Succeeds_WhenValid()
    {
        var orderItem = new OrderItem(
            _productId, _productName, _validPrice, _validQuantity, default);

        var order = new Order(_validShippingAddress, _buyerId, new OrderItem[]
        {
            orderItem
        });

        Assert.Equal(_validShippingAddress, order.ShippingAddress);

        var itemInOrder = order.Items.SingleOrDefault();
        Assert.NotNull(itemInOrder);
        Assert.Equal(orderItem.ProductId, itemInOrder.ProductId);
        Assert.Equal(orderItem.ProductUnitPrice, itemInOrder.ProductUnitPrice);
        Assert.Equal(orderItem.Quantity, itemInOrder.Quantity);
    }

    [Fact]
    public void OrderConstructor_ThrowsArgumentException_WhenOrderHasNoItems()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var order = new Order(_validShippingAddress, _buyerId, Array.Empty<OrderItem>());
        });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void OrderConstructor_ThrowsArgNullException_WhenShippingAddressEmpty(
        string shippingAddress)
    {
        var orderItem = new OrderItem(
            _productId, _productName, _validPrice, _validQuantity, default);

        Assert.Throws<ArgumentNullException>(() =>
        {
            var order = new Order(shippingAddress, _buyerId, new OrderItem[]
            {
                orderItem
            });
        });
    }

    [Fact]
    public void OrderConstructor_ThrowsDuplicateProductsException_WhenOrderContainsDuplicateProducts()
    {
        var item1 = new OrderItem(_productId, _productName,
            _validPrice, _validQuantity, default);

        var item2 = new OrderItem("PROD-2", "A Dream of Spring",
            29.99m, 5, default);

        Assert.Throws<DuplicateProductsException>(() =>
        {
            var order = new Order(_validShippingAddress, _buyerId, new OrderItem[]
            {
                item1,
                item1,
                item2,
                item2,
            });
        });
    }

    [Theory]
    [InlineData(OrderStatus.PendingSellerConfirmation)]
    [InlineData(OrderStatus.Postponed)]
    public void Cancel_Succeeds_WhenCurrentStatusIsAwaitingSellerConfirmationOrPostponed(OrderStatus status)
    {
        var order = GetTestOrder();
        SetStatus(order, status);

        order.Cancel(_orderCancelReason);

        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Equal(_orderCancelReason, order.CancelReason);
    }

    [Theory]
    [MemberData(nameof(UncancellableStatuses))]
    public void Cancel_ThrowsInvalidOpException_WhenCurrentStatusNotAwaitingSellerConfirmationNorPostponed(
        OrderStatus status)
    {
        var order = GetTestOrder();
        SetStatus(order, status);

        Assert.Throws<InvalidOperationException>(() =>
        {
            order.Cancel(_orderCancelReason);
        });

        if (status != OrderStatus.Cancelled)
        {
            Assert.NotEqual(OrderStatus.Cancelled, order.Status);
        }
        Assert.Null(order.CancelReason);
    }

    [Fact]
    public void StartPayment_Succeeds_WhenCurrentStatusIsAwaitingValidationAndAllItemStocksConfirmed()
    {
        var order = GetTestOrder();
        foreach (var item in order.Items)
        {
            item.SetStockConfirmed();
        }

        order.StartPayment();

        Assert.Equal(OrderStatus.ProcessingPayment, order.Status);
    }

    [Theory]
    [MemberData(nameof(UnpayableStatuses))]
    public void StartPayment_ThrowsInvalidOpException_WhenCurrentStatusNotAwaitingValidation(
        OrderStatus status)
    {
        var order = GetTestOrder();
        SetStatus(order, status);

        Assert.Throws<InvalidOperationException>(order.StartPayment);

        if (status != OrderStatus.ProcessingPayment)
        {
            Assert.NotEqual(OrderStatus.ProcessingPayment, order.Status);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void StartPayment_ThrowsInvalidOpException_WhenNotAllStocksConfirmed(uint itemsConfirmed)
    {
        var order = GetTestOrder();
        for (var i = 0; i < itemsConfirmed; i++)
        {
            order.Items.ElementAt(i).SetStockConfirmed();
        }

        Assert.Throws<InvalidOperationException>(order.StartPayment);
        Assert.NotEqual(OrderStatus.ProcessingPayment, order.Status);
    }

    [Fact]
    public void AwaitSellerConfirmation_Succeeds_WhenCurrentStatusIsProcessingPayment()
    {
        var order = GetTestOrder();
        SetStatus(order, OrderStatus.ProcessingPayment);

        order.RequestSellerConfirmation();

        Assert.Equal(OrderStatus.PendingSellerConfirmation, order.Status);
    }

    [Theory]
    [MemberData(nameof(SellerConfirmationUnawaitableStatuses))]
    public void AwaitSellerConfirmation_ThrowsInvalidOpException_WhenCurrentStatusNotProcessingPayment(
        OrderStatus status)
    {
        var order = GetTestOrder();
        SetStatus(order, status);

        Assert.Throws<InvalidOperationException>(order.RequestSellerConfirmation);

        if (status != OrderStatus.PendingSellerConfirmation)
        {
            Assert.NotEqual(OrderStatus.PendingSellerConfirmation, order.Status);
        }
    }

    [Fact]
    public void Ship_Succeeds_WhenCurrentStatusIsAwaitingSellerConfirmation()
    {
        var order = GetTestOrder();
        SetStatus(order, OrderStatus.PendingSellerConfirmation);

        order.Ship();

        Assert.Equal(OrderStatus.Shipping, order.Status);
    }

    [Theory]
    [MemberData(nameof(UnshippableStatuses))]
    public void Ship_ThrowsInvalidOpException_WhenCurrentStatusNotAwaitingSellerConfirmation(
        OrderStatus status)
    {
        var order = GetTestOrder();
        SetStatus(order, status);

        Assert.Throws<InvalidOperationException>(order.Ship);

        if (status != OrderStatus.Shipping)
        {
            Assert.NotEqual(OrderStatus.Shipping, order.Status);
        }
    }

    [Fact]
    public void ConfirmShipped_Succeeds_WhenCurrentStatusIsShipping()
    {
        var order = GetTestOrder();
        SetStatus(order, OrderStatus.Shipping);

        order.ConfirmShipped();

        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    [Theory]
    [MemberData(nameof(ShippedUnconfirmableStatuses))]
    public void ConfirmShipped_ThrowsInvalidOpException_WhenCurrentStatusNotShipping(
        OrderStatus status)
    {
        var order = GetTestOrder();
        SetStatus(order, status);

        Assert.Throws<InvalidOperationException>(order.ConfirmShipped);

        if (status != OrderStatus.Shipped)
        {
            Assert.NotEqual(OrderStatus.Shipped, order.Status);
        }
    }

    [Theory]
    [InlineData(OrderStatus.ProcessingPayment)]
    [InlineData(OrderStatus.PendingSellerConfirmation)]
    [InlineData(OrderStatus.Shipping)]
    public void Postpone_Succeeds_WhenCurrentStatusIsProcessingPaymentOrAwaitingConfirmationOrShipping(
        OrderStatus status)
    {
        var order = GetTestOrder();
        SetStatus(order, status);

        order.Postpone();

        Assert.Equal(OrderStatus.Postponed, order.Status);
    }

    [Theory]
    [MemberData(nameof(UnpostponableStatuses))]
    public void Postpone_ThrowsInvalidOpException_WhenCurrentStatusNotValid(
        OrderStatus status)
    {
        var order = GetTestOrder();
        SetStatus(order, status);

        Assert.Throws<InvalidOperationException>(order.Postpone);

        if (status != OrderStatus.Postponed)
        {
            Assert.NotEqual(OrderStatus.Postponed, order.Status);
        }
    }
}