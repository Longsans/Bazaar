namespace Bazaar.Contracting.Application;

public interface IContractUseCases
{
    Contract? GetById(int id);
    IEnumerable<Contract> GetByPartnerExternalId(string partnerExternalId);

    Result<ContractDto> SignPartnerForFixedPeriod(
        string partnerExternalId, int sellingPlanId, DateTime endDate);

    Result<ContractDto> SignPartnerIndefinitely(
        string partnerExternalId, int sellingPlanId);

    Result EndCurrentIndefiniteContractWithPartner(
        string partnerExternalId);

    Result ExtendCurrentFixedPeriodContractWithPartner(
        string partnerExternalId, DateTime extendedEndDate);
}
