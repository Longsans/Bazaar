namespace Bazaar.Contracting.Application;

public class ContractUseCases : IContractUseCases
{
    private readonly IContractRepository _contractRepo;
    private readonly IClientRepository _clientRepo;
    private readonly ISellingPlanRepository _sellPlanRepo;

    public ContractUseCases(
        IContractRepository contractRepo,
        IClientRepository clientRepo,
        ISellingPlanRepository sellPlanRepo)
    {
        _contractRepo = contractRepo;
        _clientRepo = clientRepo;
        _sellPlanRepo = sellPlanRepo;
    }

    public Contract? GetById(int id)
    {
        return _contractRepo.GetById(id);
    }

    public IEnumerable<Contract> GetByClientExternalId(string clientExternalId)
    {
        return _contractRepo.GetByClientExternalId(clientExternalId);
    }

    public Result<ContractDto> SignClient(
        string clientExternalId, int sellingPlanId)
    {
        var client = _clientRepo.GetWithContractsByExternalId(clientExternalId);
        if (client is null)
            return ClientNotFound;

        if (client.IsUnderContract)
            return ClientUnderContract;

        var sellingPlan = _sellPlanRepo.GetById(sellingPlanId);
        if (sellingPlan is null)
            return SellingPlanNotFound;

        var contract = new Contract(client.Id, sellingPlan.Id);
        _contractRepo.Create(contract);
        return Result.Success(new ContractDto(contract));
    }

    public Result EndCurrentContractWithClient(
        string clientExternalId)
    {
        var client = _clientRepo.GetWithContractsByExternalId(clientExternalId);
        if (client is null)
            return ClientNotFound;

        var currentContract = client.CurrentContract;
        if (currentContract is null)
            return CurrentContractNotFound;

        currentContract.End();
        _contractRepo.Update(currentContract);
        return Result.Success();
    }

    #region Helpers
    private static Result ClientNotFound
        => Result.NotFound($"Client not found.");

    private static Result ClientUnderContract
        => Result.Conflict("Client is already under contract.");

    private static Result SellingPlanNotFound
        => Result.NotFound($"Selling plan not found.");

    private static Result CurrentContractNotFound
        => Result.NotFound($"Client has no current contract.");
    #endregion
}
