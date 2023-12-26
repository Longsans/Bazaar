namespace Bazaar.Ordering.Web.Messages
{
    public class OrderItemQuery
    {
        public int Id { get; }
        public string ProductId { get; }
        public string ProductName { get; }
        public decimal ProductUnitPrice { get; }
        public uint Quantity { get; }
        public OrderItemStatus Status { get; }

        public OrderItemQuery(OrderItem item)
        {
            Id = item.Id;
            ProductId = item.ProductId;
            ProductName = item.ProductName;
            ProductUnitPrice = item.ProductUnitPrice;
            Quantity = item.Quantity;
            Status = item.Status;
        }
    }
}
