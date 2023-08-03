namespace Bazaar.Contracting.Core.Usecases;

public interface IPartnerRepository
{
    Partner? GetById(int id);
    Partner? GetByExternalId(string externalId);
    Partner Create(Partner partner);
    bool UpdateInfo(Partner update);
    bool Delete(int id);
}