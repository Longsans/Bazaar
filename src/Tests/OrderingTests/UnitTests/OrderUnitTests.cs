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
        => GetStatusValuesExcluding(OrderStatus.AwaitingSellerConfirmation, OrderStatus.Postponed);

    public static IEnumerable<object[]> UnpayableStatuses
        => GetStatusValuesExcluding(OrderStatus.AwaitingValidation);

    public static IEnumerable<object[]> SellerConfirmationUnawaitableStatuses
        => GetStatusValuesExcluding(OrderStatus.ProcessingPayment);

    public static IEnumerable<object[]> UnshippableStatuses
        => GetStatusValuesExcluding(OrderStatus.AwaitingSellerConfirmation);

    public static IEnumerable<object[]> ShippedUnconfirmableStatuses
        => GetStatusValuesExcluding(OrderStatus.Shipping);

    public static IEnumerable<object[]> UnpostponableStatuses
        => GetStatusValuesExcluding(OrderStatus.AwaitingSellerConfirmation, OrderStatus.Shipping);

    private static Order GetTestOrder()
    {
        var orderItem = new OrderItem(
            _productId, _productName, _validPrice, _validQuantity, default);

        var order = new Order(_validShippingAddress, _buyerId, new OrderItem[]
        {
            orderItem
        });

        return order;
    }

    private static void SetStatus(Order order, OrderStatus status)
    {
        typeof(Order).GetProperty(nameof(order.Status))
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
    [InlineData(OrderStatus.AwaitingSellerConfirmation)]
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
    public void StartPayment_Succeeds_WhenCurrentStatusIsAwaitingValidation()
    {
        var order = GetTestOrder();

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

    [Fact]
    public void AwaitSellerConfirmation_Succeeds_WhenCurrentStatusIsProcessingPayment()
    {
        var order = GetTestOrder();
        SetStatus(order, OrderStatus.ProcessingPayment);

        order.AwaitSellerConfirmation();

        Assert.Equal(OrderStatus.AwaitingSellerConfirmation, order.Status);
    }

    [Theory]
    [MemberData(nameof(SellerConfirmationUnawaitableStatuses))]
    public void AwaitSellerConfirmation_ThrowsInvalidOpException_WhenCurrentStatusNotProcessingPayment(
        OrderStatus status)
    {
        var order = GetTestOrder();
        SetStatus(order, status);

        Assert.Throws<InvalidOperationException>(order.AwaitSellerConfirmation);

        if (status != OrderStatus.AwaitingSellerConfirmation)
        {
            Assert.NotEqual(OrderStatus.AwaitingSellerConfirmation, order.Status);
        }
    }

    [Fact]
    public void Ship_Succeeds_WhenCurrentStatusIsAwaitingSellerConfirmation()
    {
        var order = GetTestOrder();
        SetStatus(order, OrderStatus.AwaitingSellerConfirmation);

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
    [InlineData(OrderStatus.AwaitingSellerConfirmation)]
    [InlineData(OrderStatus.Shipping)]
    public void Postpone_Succeeds_WhenCurrentStatusIsAwaitingSellerConfirmationOrShipping(
        OrderStatus status)
    {
        var order = GetTestOrder();
        SetStatus(order, status);

        order.Postpone();

        Assert.Equal(OrderStatus.Postponed, order.Status);
    }

    [Theory]
    [MemberData(nameof(UnpostponableStatuses))]
    public void Postpone_ThrowsInvalidOpException_WhenCurrentStatusNotAwaitingSellerConfirmationNorShipping(
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