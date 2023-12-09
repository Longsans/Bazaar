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
            .Include(x => x.ProductInventory)
            .SingleOrDefault(x => x.Id == id);
    }

    public Lot? GetByLotNumber(string lotNumber)
    {
        return _context.Lots
            .Include(x => x.ProductInventory)
            .SingleOrDefault(x => x.LotNumber == lotNumber);
    }

    public IEnumerable<Lot> GetManyByLotNumber(IEnumerable<string> lotNumbers)
    {
        return _context.Lots
            .Include(x => x.ProductInventory)
            .ThenInclude(p => p.SellerInventory)
            .Where(x => lotNumbers.Contains(x.LotNumber));
    }

    public Lot? GetFulfillableById(int id)
    {
        return _context.Lots
            .Include(x => x.ProductInventory)
            .SingleOrDefault(x => x.Id == id && x.UnfulfillableCategory == null);
    }

    public IEnumerable<Lot> GetUnfulfillables()
    {
        return _context.Lots
            .Include(x => x.ProductInventory)
            .ThenInclude(p => p.SellerInventory)
            .Where(x => x.UnfulfillableCategory != null);
    }

    public Lot? GetUnfulfillableById(int id)
    {
        return _context.Lots
            .Include(x => x.ProductInventory)
            .SingleOrDefault(x => x.Id == id && x.UnfulfillableCategory != null);
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
