namespace Bazaar.Contracting.Application;

public class SellingPlanUsecases : ISellingPlanUsecases
{
    private readonly ISellingPlanRepository _planRepo;

    public SellingPlanUsecases(ISellingPlanRepository planRepo)
    {
        _planRepo = planRepo;
    }

    public SellingPlan? GetById(int id)
    {
        return _planRepo.GetById(id);
    }

    public Result<SellingPlanDto> CreateSellingPlan(SellingPlanDto planDto)
    {
        if (planDto.PerSaleFee <= 0m && planDto.MonthlyFee <= 0m)
            return MonthlyAndPerSaleFeesNotPositive(
                nameof(planDto.MonthlyFee), nameof(planDto.PerSaleFee));

        if (planDto.RegularPerSaleFeePercent <= 0f)
            return RegularFeePercentNotPositive(nameof(planDto.RegularPerSaleFeePercent));

        var createdPlan = _planRepo.Create(planDto.ToNewPlan());
        return Result.Success(new SellingPlanDto(createdPlan));
    }

    public Result UpdateSellingPlan(SellingPlanDto updateDto)
    {
        var plan = _planRepo.GetById(updateDto.Id.Value);
        if (plan == null)
            return Result.NotFound("Selling plan not found.");

        if (updateDto.PerSaleFee <= 0m && updateDto.MonthlyFee <= 0m)
            return MonthlyAndPerSaleFeesNotPositive(
                nameof(updateDto.MonthlyFee), nameof(updateDto.PerSaleFee));

        if (updateDto.RegularPerSaleFeePercent <= 0f)
            return RegularFeePercentNotPositive(nameof(updateDto.RegularPerSaleFeePercent));

        plan.Name = updateDto.Name;
        plan.ChangeFees(
            updateDto.MonthlyFee,
            updateDto.PerSaleFee,
            updateDto.RegularPerSaleFeePercent);

        _planRepo.Update(plan);

        return Result.Success();
    }

    //
    // Helpers
    //
    private static Result MonthlyAndPerSaleFeesNotPositive(
        string monthlyFeePropName, string perSaleFeePropName)
    {
        return Result.Invalid(new()
        {
            new()
            {
                Identifier = monthlyFeePropName,
                ErrorMessage = PlanRequirements
                    .MonthlyFeeOrPerSaleFeeGreaterThanZeroStatement
            },
            new()
            {
                Identifier = perSaleFeePropName,
                ErrorMessage = PlanRequirements
                    .MonthlyFeeOrPerSaleFeeGreaterThanZeroStatement
            }
        });
    }

    private static Result RegularFeePercentNotPositive(
        string regularFeePercentPropName)
    {
        return Result.Invalid(new()
        {
            new()
            {
                Identifier = regularFeePercentPropName,
                ErrorMessage = PlanRequirements
                    .RegularPerSaleFeePercentGreaterThanZeroStatement
            }
        });
    }
}
