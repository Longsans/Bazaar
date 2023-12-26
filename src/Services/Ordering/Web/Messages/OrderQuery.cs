namespace Bazaar.Ordering.Web.Messages
{
    public class OrderQuery
    {
        public int Id { get; }
        public string BuyerId { get; }
        public List<OrderItemQuery> Items { get; } = new();
        public decimal Total { get; }
        public string ShippingAddress { get; }
        public OrderStatus Status { get; }
        public string? CancelReason { get; }

        public OrderQuery(Order o)
        {
            Id = o.Id;
            BuyerId = o.BuyerId;
            Items = o.Items.Select(x => new OrderItemQuery(x)).ToList();
            Total = o.Total;
            ShippingAddress = o.ShippingAddress;
            Status = o.Status;
            CancelReason = o.CancelReason;
        }
    }
}
