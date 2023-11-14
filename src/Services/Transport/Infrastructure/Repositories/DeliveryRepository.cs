namespace Bazaar.Transport.Infrastructure.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly TransportDbContext _context;

    public DeliveryRepository(TransportDbContext context)
    {
        _context = context;
    }

    public Delivery? GetById(int id)
    {
        return _context.Deliveries
            .Include(x => x.PackageItems)
            .SingleOrDefault(x => x.Id == id);
    }

    public IEnumerable<Delivery> GetIncomplete()
    {
        return _context.Deliveries
            .Include(x => x.PackageItems)
            .Where(x => x.Status != DeliveryStatus.Completed
                && x.Status != DeliveryStatus.Postponed);
    }

    public Delivery Create(Delivery delivery)
    {
        _context.Deliveries.Add(delivery);
        _context.SaveChanges();
        return delivery;
    }

    public void Update(Delivery delivery)
    {
        _context.Deliveries.Update(delivery);
        _context.SaveChanges();
    }
}
