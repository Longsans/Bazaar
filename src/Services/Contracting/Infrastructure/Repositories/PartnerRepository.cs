namespace Bazaar.Contracting.Infrastructure.Repositories;

public class PartnerRepository : IPartnerRepository
{
    private readonly ContractingDbContext _context;

    public PartnerRepository(ContractingDbContext context)
    {
        _context = context;
    }

    public Partner? GetWithContractsById(int id)
    {
        return _context.Partners
            .Include(p => p.Contracts)
            .SingleOrDefault(p => p.Id == id);
    }

    public Partner? GetWithContractsByExternalId(string externalId)
    {
        return _context.Partners
            .Include(p => p.Contracts)
            .SingleOrDefault(p => p.ExternalId == externalId);
    }

    public Partner? GetWithContractsByEmail(string email)
    {
        return _context.Partners
            .Include(p => p.Contracts)
            .SingleOrDefault(p => p.Email == email);
    }

    public Partner Create(Partner partner)
    {
        _context.Partners.Add(partner);
        _context.SaveChanges();
        return partner;
    }

    public void Update(Partner partner)
    {
        _context.Partners.Update(partner);
        _context.SaveChanges();
    }

    public bool Delete(int id)
    {
        var toRemove = _context.Partners.FirstOrDefault(p => p.Id == id);
        if (toRemove == null)
        {
            return false;
        }
        _context.Partners.Remove(toRemove);
        return true;
    }
}