namespace Bazaar.Contracting.Application;

public interface IRegisterPartnerResult
{
    static PartnerSuccessResult Success(Partner p) => new(p);
    static PartnerUnderEighteenError PartnerUnderEighteen => new();
    static PartnerEmailAlreadyExistsError PartnerEmailAlreadyExists => new();
}

public interface IUpdatePartnerInfoResult
{
    static PartnerSuccessResult Success => new();
    static PartnerUnderEighteenError PartnerUnderEighteen => new();
    static PartnerEmailAlreadyExistsError PartnerEmailAlreadyExists => new();
    static PartnerNotFoundError PartnerNotFound => new();
}

public class PartnerSuccessResult
    : IRegisterPartnerResult,
    IUpdatePartnerInfoResult
{
    public Partner? Partner { get; set; }

    public PartnerSuccessResult() { }

    public PartnerSuccessResult(Partner partner)
    {
        Partner = partner;
    }
}

public class PartnerUnderEighteenError
    : IRegisterPartnerResult,
    IUpdatePartnerInfoResult
{ }

public class PartnerEmailAlreadyExistsError
    : IRegisterPartnerResult,
    IUpdatePartnerInfoResult
{ }

public class PartnerNotFoundError
    : IUpdatePartnerInfoResult,
    ISignFixedPeriodContractResult,
    ISignIndefiniteContractResult,
    IEndContractResult,
    IExtendContractResult
{ }
