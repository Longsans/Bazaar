namespace Bazaar.Contracting.Application;

public interface IContractUseCases
{
    Contract? GetById(int id);
    IEnumerable<Contract> GetByClientExternalId(string clientExternalId);

    Result<ContractDto> SignClient(
        string clientExternalId, int sellingPlanId);

    Result EndCurrentContractWithClient(
        string clientExternalId);
}
