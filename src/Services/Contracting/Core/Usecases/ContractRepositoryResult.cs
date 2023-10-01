namespace Bazaar.Contracting.Core.Usecases;

public abstract class ContractRepositoryResult { }

// Concrete results
public class ContractSuccessResult :
    ContractRepositoryResult,
    ICreateFixedPeriodResult,
    ICreateIndefiniteResult,
    IEndContractResult,
    IExtendContractResult
{
    public Contract? Contract;

    public ContractSuccessResult() { }

    public ContractSuccessResult(Contract contract) { Contract = contract; }
}

public class ContractEndDateBeforeCurrentDate :
    ContractRepositoryResult, ICreateFixedPeriodResult, ICreateIndefiniteResult
{ }

public class PartnerNotFoundError :
    ContractRepositoryResult, ICreateFixedPeriodResult, ICreateIndefiniteResult, IEndContractResult
{ }

public class PartnerUnderContractError :
    ContractRepositoryResult, ICreateFixedPeriodResult, ICreateIndefiniteResult
{ }

public class SellingPlanNotFoundError :
    ContractRepositoryResult, ICreateFixedPeriodResult, ICreateIndefiniteResult
{ }

public class ContractNotFoundError :
    ContractRepositoryResult, IEndContractResult, IExtendContractResult
{ }

public class ContractNotIndefiniteError :
    ContractRepositoryResult, IEndContractResult
{ }

public class EndDateNotAfterOldEndDateError :
    ContractRepositoryResult, IExtendContractResult
{ }

public class ContractNotFixedPeriodError :
    ContractRepositoryResult, IExtendContractResult
{ }

public class ContractEndedError :
    ContractRepositoryResult, IExtendContractResult
{ }


// Method return interfaces
public interface ICreateFixedPeriodResult
{
    static ContractSuccessResult Success => new();
    static ContractEndDateBeforeCurrentDate ContractEndDateBeforeCurrentDate => new();
    static PartnerNotFoundError PartnerNotFoundError => new();
    static PartnerUnderContractError PartnerUnderContractError => new();
    static SellingPlanNotFoundError SellingPlanNotFoundError => new();
}

public interface ICreateIndefiniteResult
{
    static ContractSuccessResult Success => new();
    static ContractEndDateBeforeCurrentDate ContractStartDateInPastOrAfterEndDateError => new();
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
    static EndDateNotAfterOldEndDateError EndDateNotAfterOldEndDateError => new();
    static ContractNotFoundError ContractNotFoundError => new();
    static ContractNotFixedPeriodError ContractNotFixedPeriodError => new();
    static ContractEndedError ContractEndedError => new();
}