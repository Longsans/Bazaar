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
            .FirstOrDefault(p => p.Id == id);
    }

    public Partner? GetWithContractsByExternalId(string externalId)
    {
        return _context.Partners
            .Include(p => p.Contracts)
            .FirstOrDefault(p => p.ExternalId == externalId);
    }

    public Partner Create(Partner partner)
    {
        _context.Partners.Add(partner);
        _context.SaveChanges();
        return partner;
    }

    public bool UpdateInfo(Partner update)
    {
        var partner = _context.Partners.FirstOrDefault(p => p.Id == update.Id);
        if (partner == null)
        {
            return false;
        }

        partner.FirstName = update.FirstName;
        partner.LastName = update.LastName;
        partner.Email = update.Email;
        partner.PhoneNumber = update.PhoneNumber;
        partner.DateOfBirth = update.DateOfBirth;
        partner.Gender = update.Gender;
        _context.SaveChanges();

        return true;
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