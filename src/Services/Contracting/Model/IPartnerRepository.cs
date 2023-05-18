namespace Bazaar.Contracting.Model;

public interface IPartnerRepository
{
    Partner? GetById(int id);
    Partner? GetByExternalId(string externalId);
}