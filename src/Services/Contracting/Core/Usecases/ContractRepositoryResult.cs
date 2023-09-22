namespace Bazaar.Contracting.Core.Usecases;

public abstract class ContractRepositoryResult { }

// Concrete results
public class ContractSuccessResult :
    ContractRepositoryResult, ICreateFixedPeriodResult, ICreateIndefiniteResult, IEndContractResult
{
    public Contract? Contract;

    public ContractSuccessResult() { }

    public ContractSuccessResult(Contract contract) { Contract = contract; }
}

public class ContractStartDateInPastOrAfterEndDateError :
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
    ContractRepositoryResult, IEndContractResult
{ }

public class ContractNotIndefiniteError :
    ContractRepositoryResult, IEndContractResult
{ }


// Method return interfaces
public interface ICreateFixedPeriodResult
{
    static ContractSuccessResult Success => new();
    static ContractStartDateInPastOrAfterEndDateError ContractStartDateInPastOrAfterEndDateError => new();
    static PartnerNotFoundError PartnerNotFoundError => new();
    static PartnerUnderContractError PartnerUnderContractError => new();
    static SellingPlanNotFoundError SellingPlanNotFoundError => new();
}

public interface ICreateIndefiniteResult
{
    static ContractSuccessResult Success => new();
    static ContractStartDateInPastOrAfterEndDateError ContractStartDateInPastOrAfterEndDateError => new();
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
