namespace Bazaar.Contracting.Core.Usecases;

public interface IContractRepository
{
    Contract? GetById(int id);
    void CreateFixedPeriod(Contract contract);
    void CreateIndefinite(Contract contract);
    void EndContract(int id);
}
