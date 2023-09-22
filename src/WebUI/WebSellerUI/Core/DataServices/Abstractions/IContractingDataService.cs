namespace WebSellerUI.DataServices;

using ContractCollectionResult = ServiceCallResult<IEnumerable<Contract>>;
using ContractResult = ServiceCallResult<Contract>;
using PartnerResult = ServiceCallResult<Partner>;

public interface IContractingDataService
{
    Task<ContractCollectionResult> GetContractsByPartnerId(string partnerId);
    Task<PartnerResult> GetPartnerById(string partnerId);

    Task<ContractResult> SignFixedPeriodContract(
        string partnerId, FixedPeriodContractCreateCommand command);

    Task<ContractResult> SignIndefiniteContract(
        string partnerId, IndefiniteContractCreateCommand command);

    Task<ContractResult> EndCurrentIndefiniteContract(string partnerId);
}
