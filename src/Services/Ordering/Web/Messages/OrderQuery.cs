namespace Bazaar.Ordering.Web.Messages
{
    public class OrderQuery
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public List<OrderItemQuery> Items { get; set; } = new();
        public decimal Total { get; set; }
        public string ShippingAddress { get; set; }
        public string Status { get; set; }
        public string? CancelReason { get; set; }

        public OrderQuery(Order o)
        {
            Id = o.Id;
            BuyerId = o.BuyerId;
            Items = o.Items.Select(x => new OrderItemQuery(x)).ToList();
            Total = o.Total;
            ShippingAddress = o.ShippingAddress;
            Status = Enum.GetName(o.Status)!;
            CancelReason = o.CancelReason;
        }
    }
}
