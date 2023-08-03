namespace Bazaar.ApiGateways.WebBff.Core.Model
{
    public class OrderQuery
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string BuyerId { get; set; }
        public List<OrderItemQuery> Items { get; set; } = new();
        public string Status { get; set; }
    }
}
