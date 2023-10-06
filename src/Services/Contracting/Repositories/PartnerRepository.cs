namespace Bazaar.Contracting.Repositories;

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

    public Partner? UpdateInfoByExternalId(Partner update)
    {
        var partner = _context.Partners
            .SingleOrDefault(p => p.ExternalId == update.ExternalId);

        if (partner != null)
        {
            partner.FirstName = update.FirstName;
            partner.LastName = update.LastName;
            partner.Email = update.Email;
            partner.PhoneNumber = update.PhoneNumber;
            partner.DateOfBirth = update.DateOfBirth;
            partner.Gender = update.Gender;
            _context.SaveChanges();
        }

        return partner;
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