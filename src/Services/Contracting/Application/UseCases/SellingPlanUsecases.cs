namespace Bazaar.Contracting.Application;

public class SellingPlanUseCases : ISellingPlanUseCases
{
    private readonly ISellingPlanRepository _planRepo;

    public SellingPlanUseCases(ISellingPlanRepository planRepo)
    {
        _planRepo = planRepo;
    }

    public SellingPlan? GetById(int id)
    {
        return _planRepo.GetById(id);
    }

    public Result<SellingPlanDto> CreateSellingPlan(SellingPlanDto planDto)
    {
        try
        {
            var createdPlan = _planRepo.Create(planDto.ToNewPlan());
            return Result.Success(new SellingPlanDto(createdPlan));
        }
        catch (MonthlyAndPerSaleFeesNotPositiveException)
        {
            return MonthlyAndPerSaleFeesNotPositive(
                nameof(planDto.MonthlyFee), nameof(planDto.PerSaleFee));
        }
        catch (RegularPerSaleFeePercentNotPositiveException)
        {
            return RegularFeePercentNotPositive(
                nameof(planDto.RegularPerSaleFeePercent));
        }
    }

    public Result UpdateSellingPlan(SellingPlanDto updateDto)
    {
        var plan = _planRepo.GetById(updateDto.Id.Value);
        if (plan == null)
            return Result.NotFound("Selling plan not found.");

        plan.Name = updateDto.Name;
        try
        {
            plan.ChangeFees(
                updateDto.MonthlyFee,
                updateDto.PerSaleFee,
                updateDto.RegularPerSaleFeePercent);
        }
        catch (MonthlyAndPerSaleFeesNotPositiveException)
        {
            return MonthlyAndPerSaleFeesNotPositive(
                nameof(updateDto.MonthlyFee), nameof(updateDto.PerSaleFee));
        }
        catch (RegularPerSaleFeePercentNotPositiveException)
        {
            return RegularFeePercentNotPositive(
                nameof(updateDto.RegularPerSaleFeePercent));
        }

        _planRepo.Update(plan);
        return Result.Success();
    }

    #region Helpers
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
    #endregion
}
