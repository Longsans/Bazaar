namespace Bazaar.Contracting.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ContractingDbContext _context;

    public ClientRepository(ContractingDbContext context)
    {
        _context = context;
    }

    public Client? GetWithContractsAndPlanById(int id)
    {
        return _context.Clients
            .Include(c => c.Contracts)
            .Include(c => c.SellingPlan)
            .SingleOrDefault(c => c.Id == id);
    }

    public Client? GetWithContractsAndPlanByExternalId(string externalId)
    {
        return _context.Clients
            .Include(c => c.Contracts)
            .Include(c => c.SellingPlan)
            .SingleOrDefault(c => c.ExternalId == externalId);
    }

    public Client? GetWithContractsAndPlanByEmailAddress(string emailAddress)
    {
        return _context.Clients
            .Include(c => c.Contracts)
            .Include(c => c.SellingPlan)
            .SingleOrDefault(c => c.EmailAddress == emailAddress);
    }

    public Client Create(Client client)
    {
        _context.Clients.Add(client);
        _context.SaveChanges();
        return client;
    }

    public void Update(Client client)
    {
        _context.Clients.Update(client);
        _context.SaveChanges();
    }

    public bool Delete(int id)
    {
        var toRemove = _context.Clients.FirstOrDefault(c => c.Id == id);
        if (toRemove == null)
        {
            return false;
        }
        _context.Clients.Remove(toRemove);
        return true;
    }
}