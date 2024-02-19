namespace Bazaar.ShopperInfo.Infrastructure.Repositories;

public class ShopperRepository : IShopperRepository
{
    private readonly ShopperInfoDbContext _context;

    public ShopperRepository(ShopperInfoDbContext context)
    {
        _context = context;
    }

    public Shopper? GetById(int id)
    {
        return _context.Shoppers.SingleOrDefault(s => s.Id == id);
    }

    public Shopper? GetByExternalId(string externalId)
    {
        return _context.Shoppers.SingleOrDefault(s => s.ExternalId == externalId);
    }

    public Shopper? GetByEmailAddress(string emailAddress)
    {
        return _context.Shoppers.SingleOrDefault(x => x.EmailAddress == emailAddress);
    }

    public Shopper Create(Shopper shopper)
    {
        _context.Shoppers.Add(shopper);
        _context.SaveChanges();
        return shopper;
    }

    public void Update(Shopper update)
    {
        _context.Shoppers.Update(update);
        _context.SaveChanges();
    }
}
