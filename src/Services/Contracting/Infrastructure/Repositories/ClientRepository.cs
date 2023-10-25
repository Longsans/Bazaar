namespace Bazaar.Contracting.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ContractingDbContext _context;

    public ClientRepository(ContractingDbContext context)
    {
        _context = context;
    }

    public Client? GetWithContractsById(int id)
    {
        return _context.Clients
            .Include(p => p.Contracts)
            .SingleOrDefault(p => p.Id == id);
    }

    public Client? GetWithContractsByExternalId(string externalId)
    {
        return _context.Clients
            .Include(p => p.Contracts)
            .SingleOrDefault(p => p.ExternalId == externalId);
    }

    public Client? GetWithContractsByEmailAddress(string email)
    {
        return _context.Clients
            .Include(p => p.Contracts)
            .SingleOrDefault(p => p.EmailAddress == email);
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
        var toRemove = _context.Clients.FirstOrDefault(p => p.Id == id);
        if (toRemove == null)
        {
            return false;
        }
        _context.Clients.Remove(toRemove);
        return true;
    }
}