namespace Bazaar.Contracting.Core.DomainLogic;

// Concrete results
public class ContractSuccessResult :
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
    ISignFixedPeriodContractResult, ISignIndefiniteContractResult
{ }

public class PartnerUnderContractError :
    ISignFixedPeriodContractResult, ISignIndefiniteContractResult
{ }

public class ContractSellingPlanNotFoundError :
    ISignFixedPeriodContractResult, ISignIndefiniteContractResult
{ }

public class ContractNotFoundError :
    IEndContractResult, IExtendContractResult
{ }

public class ContractNotIndefiniteError :
    IEndContractResult
{ }

public class EndDateNotAfterOldEndDateError :
    IExtendContractResult
{ }

public class ContractNotFixedPeriodError :
    IExtendContractResult
{ }

// Method return interfaces
public interface ISignFixedPeriodContractResult
{
    static ContractSuccessResult Success(Contract c) => new(c);
    static ContractEndDateBeforeCurrentDate ContractEndDateBeforeCurrentDate => new();
    static PartnerNotFoundError PartnerNotFoundError => new();
    static PartnerUnderContractError PartnerUnderContractError => new();
    static ContractSellingPlanNotFoundError SellingPlanNotFoundError => new();
}

public interface ISignIndefiniteContractResult
{
    static ContractSuccessResult Success(Contract c) => new(c);
    static PartnerNotFoundError PartnerNotFoundError => new();
    static PartnerUnderContractError PartnerUnderContractError => new();
    static ContractSellingPlanNotFoundError SellingPlanNotFoundError => new();
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