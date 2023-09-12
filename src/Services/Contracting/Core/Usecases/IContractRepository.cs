namespace Bazaar.Contracting.Core.Usecases;

public interface IContractRepository
{
    Contract? GetById(int id);
    IEnumerable<Contract> GetByPartnerId(string partnerId);
    ICreateFixedPeriodResult CreateFixedPeriod(Contract contract);
    ICreateIndefiniteResult CreateIndefinite(Contract contract);
    IEndContractResult EndIndefiniteContract(int id);
}
