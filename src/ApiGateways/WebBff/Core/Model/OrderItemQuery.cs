namespace Bazaar.ApiGateways.WebBff.Core.Model
{
    public class OrderItemQuery
    {
        public int Id { get; set; }
        public string ProductExternalId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductUnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
