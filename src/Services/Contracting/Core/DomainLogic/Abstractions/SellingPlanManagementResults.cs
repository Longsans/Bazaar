namespace Bazaar.Contracting.Core.DomainLogic;

public interface ICreateSellingPlanResult
{
    static SellingPlanSuccessResult Success(SellingPlan p) => new(p);
    static PerSaleFeeAndMonthlyFeeNotPositiveError PerSaleFeeAndMonthlyFeeNotPositive => new();
    static RegularPerSaleFeeNotPositiveError RegularPerSaleFeeNotPositive => new();
}

public interface IUpdateSellingPlanResult
{
    static SellingPlanSuccessResult Success(SellingPlan p) => new(p);
    static PerSaleFeeAndMonthlyFeeNotPositiveError PerSaleFeeAndMonthlyFeeNotPositive => new();
    static RegularPerSaleFeeNotPositiveError RegularPerSaleFeeNotPositive => new();
    static SellingPlanNotFoundError SellingPlanNotFound => new();
}

public class SellingPlanSuccessResult
    : ICreateSellingPlanResult,
    IUpdateSellingPlanResult
{
    public SellingPlan Plan { get; set; }

    public SellingPlanSuccessResult(SellingPlan plan)
    {
        Plan = plan;
    }
}

public class PerSaleFeeAndMonthlyFeeNotPositiveError
    : ICreateSellingPlanResult,
    IUpdateSellingPlanResult
{ }

public class RegularPerSaleFeeNotPositiveError
    : ICreateSellingPlanResult,
    IUpdateSellingPlanResult
{ }

public class SellingPlanNotFoundError
    : IUpdateSellingPlanResult,
    ISignFixedPeriodContractResult,
    ISignIndefiniteContractResult
{ }