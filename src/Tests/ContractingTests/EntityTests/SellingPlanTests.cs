namespace ContractingTests.EntityTests;

public class SellingPlanTests
{
    private readonly SellingPlan _existingPlan = new("Individual", 30m, 0.99m, 0.01f);

    [Fact]
    public void ChangeFees_Succeeds_WhenValid()
    {
        var monthlyFee = 40m;
        var perSaleFee = 0.79m;
        var regularFeePercent = 0.02f;

        _existingPlan.ChangeFees(monthlyFee, perSaleFee, regularFeePercent);

        Assert.Equal(monthlyFee, _existingPlan.MonthlyFee);
        Assert.Equal(perSaleFee, _existingPlan.PerSaleFee);
        Assert.Equal(regularFeePercent, _existingPlan.RegularPerSaleFeePercent);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-40, 0)]
    [InlineData(0, -0.99)]
    [InlineData(-40, -0.99)]
    public void ChangeFees_ThrowsException_WhenMonthlyFeeAndPerSaleFeeNotPositive(
        decimal monthlyFee, decimal perSaleFee)
    {
        var regularFeePercent = 0.02f;

        Assert.Throws<MonthlyAndPerSaleFeesNotPositiveException>(
            () => _existingPlan.ChangeFees(monthlyFee, perSaleFee, regularFeePercent));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void ChangeFees_ThrowsException_WhenRegularFeePercentNotPositive(float regularFeePercent)
    {
        var monthlyFee = 40m;
        var perSaleFee = 0.79m;

        Assert.Throws<RegularPerSaleFeePercentNotPositiveException>(
            () => _existingPlan.ChangeFees(monthlyFee, perSaleFee, regularFeePercent));
    }
}
