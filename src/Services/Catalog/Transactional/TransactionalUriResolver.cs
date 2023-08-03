namespace Bazaar.Catalog.Infrastructure.Transactional
{
    public class TransactionalUriResolver : IResourceLocationResolver
    {
        private readonly string CATALOG_SERVICE_URI;
        private readonly string CUSTOMER_SERVICE_URI;
        private readonly string PARTNER_SERVICE_URI;
        private readonly string ORDER_SERVICE_URI;
        private readonly string BASKET_SERVICE_URI;

        public TransactionalUriResolver(IConfiguration config)
        {
            CATALOG_SERVICE_URI = config["CatalogServiceUri"];
            CUSTOMER_SERVICE_URI = config["CustomerServiceUri"];
            PARTNER_SERVICE_URI = config["PartnerServiceUri"];
            ORDER_SERVICE_URI = config["OrderServiceUri"];
            BASKET_SERVICE_URI = config["BasketServiceUri"];
        }

        public IList<string> GetResourceNodesFromIndexes(IList<string> indexes)
        {
            var indexTypes = indexes.GroupBy(index => index[..4]).Select(g => g.Key).ToList();
            List<string> nodes = new(3);
            foreach (var indexType in indexTypes)
            {
                nodes.Add(indexType switch
                {
                    "PROD" => CATALOG_SERVICE_URI,
                    "CUST" => CUSTOMER_SERVICE_URI,
                    "PNER" => PARTNER_SERVICE_URI,
                    "ORDR" => ORDER_SERVICE_URI,
                    "BASK" => BASKET_SERVICE_URI,
                    // other index types + service addresses
                    _ => throw new Exception($"Unhandled index type: {indexType}")
                });
            }
            return nodes;
        }
    }
}
