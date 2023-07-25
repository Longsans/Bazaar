namespace Bazaar.ApiGateways.WebBff.Core.Model
{
    public class OrderQuery
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string BuyerExternalId { get; set; }
        public List<OrderItemQuery> Items { get; set; } = new();
        public string Status { get; set; }
    }
}
