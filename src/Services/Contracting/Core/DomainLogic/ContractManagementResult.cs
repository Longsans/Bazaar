namespace Bazaar.Contracting.Core.DomainLogic;

public abstract class ContractManagementResult { }

// Concrete results
public class ContractSuccessResult :
    ContractManagementResult,
    ISignFixedPeriodContractResult,
    ISignIndefiniteContractResult,
    IEndContractResult,
    IExtendContractResult
{
    public Contract? Contract;

    public ContractSuccessResult() { }

    public ContractSuccessResult(Contract contract) { Contract = contract; }
}

public class ContractEndDateBeforeCurrentDate :
    ContractManagementResult, ISignFixedPeriodContractResult, ISignIndefiniteContractResult
{ }

public class PartnerNotFoundError :
    ContractManagementResult,
    ISignFixedPeriodContractResult,
    ISignIndefiniteContractResult,
    IEndContractResult,
    IExtendContractResult
{ }

public class PartnerUnderContractError :
    ContractManagementResult, ISignFixedPeriodContractResult, ISignIndefiniteContractResult
{ }

public class SellingPlanNotFoundError :
    ContractManagementResult, ISignFixedPeriodContractResult, ISignIndefiniteContractResult
{ }

public class ContractNotFoundError :
    ContractManagementResult, IEndContractResult, IExtendContractResult
{ }

public class ContractNotIndefiniteError :
    ContractManagementResult, IEndContractResult
{ }

public class EndDateNotAfterOldEndDateError :
    ContractManagementResult, IExtendContractResult
{ }

public class ContractNotFixedPeriodError :
    ContractManagementResult, IExtendContractResult
{ }

// Method return interfaces
public interface ISignFixedPeriodContractResult
{
    static ContractSuccessResult Success(Contract c) => new(c);
    static ContractEndDateBeforeCurrentDate ContractEndDateBeforeCurrentDate => new();
    static PartnerNotFoundError PartnerNotFoundError => new();
    static PartnerUnderContractError PartnerUnderContractError => new();
    static SellingPlanNotFoundError SellingPlanNotFoundError => new();
}

public interface ISignIndefiniteContractResult
{
    static ContractSuccessResult Success(Contract c) => new(c);
    static PartnerNotFoundError PartnerNotFoundError => new();
    static PartnerUnderContractError PartnerUnderContractError => new();
    static SellingPlanNotFoundError SellingPlanNotFoundError => new();
}

public interface IEndContractResult
{
    static ContractSuccessResult Success(Contract c) => new(c);
    static PartnerNotFoundError PartnerNotFoundError => new();
    static ContractNotFoundError ContractNotFoundError => new();
    static ContractNotIndefiniteError ContractNotIndefiniteError => new();
}

public interface IExtendContractResult
{
    static ContractSuccessResult Success(Contract c) => new(c);
    static PartnerNotFoundError PartnerNotFoundError => new();
    static ContractNotFoundError ContractNotFoundError => new();
    static EndDateNotAfterOldEndDateError EndDateNotAfterOldEndDateError => new();
    static ContractNotFixedPeriodError ContractNotFixedPeriodError => new();
}