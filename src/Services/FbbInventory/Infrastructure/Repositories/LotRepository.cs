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

    public FulfillableLot? GetFulfillableById(int id)
    {
        return _context.FulfillableLots
            .Include(x => x.ProductInventory)
            .SingleOrDefault(x => x.Id == id);
    }

    public IEnumerable<UnfulfillableLot> GetUnfulfillables()
    {
        return _context.UnfulfillableLots
            .Include(x => x.ProductInventory)
            .ThenInclude(p => p.SellerInventory);
    }

    public UnfulfillableLot? GetUnfulfillableById(int id)
    {
        return _context.UnfulfillableLots
            .Include(x => x.ProductInventory)
            .SingleOrDefault(x => x.Id == id);
    }

    public FulfillableLot CreateFulfillable(FulfillableLot lot)
    {
        _context.FulfillableLots.Add(lot);
        _context.SaveChanges();
        return lot;
    }

    public UnfulfillableLot CreateUnfulfillable(UnfulfillableLot lot)
    {
        _context.UnfulfillableLots.Add(lot);
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
