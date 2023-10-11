namespace Bazaar.Contracting.Application;

// Concrete results
public class ContractSuccessResult :
    ISignFixedPeriodContractResult,
    ISignIndefiniteContractResult,
    IEndContractResult,
    IExtendContractResult
{
    public Contract Contract;

    public ContractSuccessResult(Contract contract) { Contract = contract; }
}

public class ContractEndDateBeforeCurrentDate :
    ISignFixedPeriodContractResult, ISignIndefiniteContractResult
{ }

public class PartnerUnderContractError :
    ISignFixedPeriodContractResult, ISignIndefiniteContractResult
{
    public Contract Contract;

    public PartnerUnderContractError(Contract contract) { Contract = contract; }
}

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
    static PartnerUnderContractError PartnerUnderContractError(Contract c) => new(c);
    static SellingPlanNotFoundError SellingPlanNotFoundError => new();
}

public interface ISignIndefiniteContractResult
{
    static ContractSuccessResult Success(Contract c) => new(c);
    static PartnerNotFoundError PartnerNotFoundError => new();
    static PartnerUnderContractError PartnerUnderContractError(Contract c) => new(c);
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