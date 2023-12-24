namespace Bazaar.FbbInventory.Infrastructure.Repositories;

public class LotRepository : ILotRepository
{
    private readonly FbbInventoryDbContext _context;

    public LotRepository(FbbInventoryDbContext context)
    {
        _context = context;
    }

    public Lot? GetById(int id)
    {
        return _context.Lots
            .Include(x => x.ProductInventory.Lots)
            .Include(x => x.ProductInventory.SellerInventory)
            .SingleOrDefault(x => x.Id == id);
    }

    public Lot? GetByLotNumber(string lotNumber)
    {
        return _context.Lots
            .Include(x => x.ProductInventory.Lots)
            .Include(x => x.ProductInventory.SellerInventory)
            .SingleOrDefault(x => x.LotNumber == lotNumber);
    }

    public IEnumerable<Lot> GetUnfulfillables()
    {
        return _context.Lots
            .Include(x => x.ProductInventory.Lots)
            .Include(x => x.ProductInventory.SellerInventory)
            .Where(x => x.IsUnitsUnfulfillable);
    }

    public Lot Create(Lot lot)
    {
        _context.Lots.Add(lot);
        _context.SaveChanges();
        return lot;
    }

    public void Update(Lot lot)
    {
        _context.Lots.Update(lot);
        _context.SaveChanges();
    }

    public void UpdateRange(IEnumerable<Lot> lots)
    {
        _context.Lots.UpdateRange(lots);
        _context.SaveChanges();
    }
}
