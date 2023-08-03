namespace Bazaar.Ordering.Dto
{
    public class OrderItemQuery
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductUnitPrice { get; set; }
        public int Quantity { get; set; }

        public OrderItemQuery(OrderItem item)
        {
            Id = item.Id;
            ProductId = item.ProductId;
            ProductName = item.ProductName;
            ProductUnitPrice = item.ProductUnitPrice;
            Quantity = item.Quantity;
        }
    }
}
