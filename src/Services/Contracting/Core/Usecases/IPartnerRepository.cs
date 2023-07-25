namespace Bazaar.Contracting.Core.Usecases;

public interface IPartnerRepository
{
    Partner? GetById(int id);
    Partner? GetByExternalId(string externalId);
}