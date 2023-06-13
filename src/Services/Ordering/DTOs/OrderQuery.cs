namespace Bazaar.Ordering.DTOs
{
    public class OrderQuery
    {
        public int Id { get; set; }
        public string BuyerExternalId { get; set; }
        public List<OrderItemQuery> Items { get; set; } = new();
        public string Status { get; set; }

        public OrderQuery(Order o)
        {
            Id = o.Id;
            BuyerExternalId = o.BuyerExternalId;
            Items = o.Items.Select(x => new OrderItemQuery(x)).ToList();
            Status = Enum.GetName(o.Status)!;
        }
    }
}
