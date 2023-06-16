namespace Bazaar.Ordering.Dto
{
    public class OrderQuery
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string BuyerExternalId { get; set; }
        public List<OrderItemQuery> Items { get; set; } = new();
        public string Status { get; set; }

        public OrderQuery(Order o)
        {
            Id = o.Id;
            ExternalId = o.ExternalId;
            BuyerExternalId = o.BuyerExternalId;
            Items = o.Items.Select(x => new OrderItemQuery(x)).ToList();
            Status = Enum.GetName(o.Status)!;
        }
    }
}
