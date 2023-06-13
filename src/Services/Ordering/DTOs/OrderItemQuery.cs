namespace Bazaar.Ordering.DTOs
{
    public class OrderItemQuery
    {
        public int Id { get; set; }
        public string ProductExternalId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductUnitPrice { get; set; }
        public int Quantity { get; set; }

        public OrderItemQuery(OrderItem item)
        {
            Id = item.Id;
            ProductExternalId = item.ProductExternalId;
            ProductName = item.ProductName;
            ProductUnitPrice = item.ProductUnitPrice;
            Quantity = item.Quantity;
        }
    }
}
