namespace Bazaar.Contracting.Core.DomainLogic;

public interface IContractRepository
{
    Contract? GetById(int id);
    IEnumerable<Contract> GetByPartnerExternalId(string partnerId);
    Contract? Create(Contract contract);
    Contract? UpdateEndDate(int id, DateTime endDate);
}