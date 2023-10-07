namespace Bazaar.Contracting.Core.DomainLogic;

public interface IContractManager
{
    Contract? GetById(int id);
    IEnumerable<Contract> GetByPartnerExternalId(string partnerExternalId);

    ISignFixedPeriodContractResult SignPartnerForFixedPeriod(
        string partnerExternalId, int sellingPlanId, DateTime endDate);

    ISignIndefiniteContractResult SignPartnerIndefinitely(
        string partnerExternalId, int sellingPlanId);

    IEndContractResult EndCurrentIndefiniteContractWithPartner(
        string partnerExternalId);

    IExtendContractResult ExtendCurrentFixedPeriodContractWithPartner(
        string partnerExternalId, DateTime extendedEndDate);
}
