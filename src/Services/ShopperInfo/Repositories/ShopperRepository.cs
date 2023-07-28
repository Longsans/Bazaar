namespace Bazaar.ShopperInfo.Repositories
{
    public class ShopperRepository : IShopperRepository
    {
        private readonly List<Shopper> _shoppers;
        private int _nextId => _shoppers.Count + 1;

        public ShopperRepository()
        {
            _shoppers = new List<Shopper>
            {
                new Shopper
                {
                    Id = 1,
                    ExternalId = "SPER-1",
                    FirstName = "Long",
                    LastName = "Do",
                    Email = "longdo@thegreedycompany.com",
                    PhoneNumber = "0901111111",
                    Gender = Gender.Male,
                    DateOfBirth = new DateTime(2001, 12, 11),
                }
            };
        }

        public Shopper? GetById(int id)
        {
            return _shoppers.FirstOrDefault(s => s.Id == id);
        }

        public Shopper? GetByExternalId(string externalId)
        {
            return _shoppers.FirstOrDefault(s => s.ExternalId == externalId);
        }

        public Shopper Create(Shopper shopper)
        {
            shopper.Id = _nextId;
            shopper.ExternalId = $"SPER-{shopper.Id}";
            _shoppers.Add(shopper);
            return shopper;
        }

        public bool Update(Shopper update)
        {
            var shopper = _shoppers.FirstOrDefault(s => s.Id == update.Id);
            if (shopper == null)
            {
                return false;
            }
            _shoppers.Remove(shopper);
            _shoppers.Add(update);
            return true;
        }

        public bool Delete(int id)
        {
            var toRemove = _shoppers.FirstOrDefault(s => s.Id == id);
            if (toRemove == null)
            {
                return false;
            }
            _shoppers.Remove(toRemove);
            return true;
        }
    }
}
