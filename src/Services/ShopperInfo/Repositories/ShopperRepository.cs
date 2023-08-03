namespace Bazaar.ShopperInfo.Repositories
{
    public class ShopperRepository : IShopperRepository
    {
        private readonly ShopperInfoDbContext _context;

        public ShopperRepository(ShopperInfoDbContext context)
        {
            _context = context;
        }

        public Shopper? GetById(int id)
        {
            return _context.Shoppers.FirstOrDefault(s => s.Id == id);
        }

        public Shopper? GetByExternalId(string externalId)
        {
            return _context.Shoppers.FirstOrDefault(s => s.ExternalId == externalId);
        }

        public Shopper Register(Shopper shopper)
        {
            _context.Shoppers.Add(shopper);
            _context.SaveChanges();
            return shopper;
        }

        public bool UpdateInfo(Shopper update)
        {
            var shopper = _context.Shoppers.FirstOrDefault(s => s.Id == update.Id);
            if (shopper == null)
            {
                return false;
            }

            var shopperEntry = _context.Entry(shopper);
            shopperEntry.CurrentValues.SetValues(update);
            _context.SaveChanges();

            return true;
        }

        public bool Delete(int id)
        {
            var toRemove = _context.Shoppers.FirstOrDefault(s => s.Id == id);
            if (toRemove == null)
            {
                return false;
            }

            _context.Shoppers.Remove(toRemove);
            _context.SaveChanges();
            return true;
        }
    }
}
