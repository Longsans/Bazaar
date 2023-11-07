namespace ContractingTests.UnitTests;

public class SellingPlanUnitTests
{
    private readonly SellingPlan _existingPlan = new("Individual", 30m, 0.99m, 0.01f);

    [Fact]
    public void Constructor_Succeeds_WhenValid()
    {
        var monthlyFee = 40m;
        var perSaleFee = 0m;
        var regularFeePercent = 0.01f;
        var plan = new SellingPlan("Business", monthlyFee,
            perSaleFee, regularFeePercent);

        Assert.Equal(monthlyFee, plan.MonthlyFee);
        Assert.Equal(perSaleFee, plan.PerSaleFee);
        Assert.Equal(regularFeePercent, plan.RegularPerSaleFeePercent);
    }

    [Fact]
    public void Constructor_ThrowsException_WhenMonthlyFeeAndPerSaleFeeAreZero()
    {
        var monthlyFee = 0m;
        var perSaleFee = 0m;
        var regularFeePercent = 0.01f;

        Assert.Throws<MonthlyAndPerSaleFeesEqualZeroException>(() =>
        {
            var plan = new SellingPlan("Business", monthlyFee,
                perSaleFee, regularFeePercent);
        });
    }

    [Theory]
    [InlineData(40, -1)]
    [InlineData(-40, 1)]
    [InlineData(-40, -1)]
    public void Constructor_ThrowsArgOutOfRangeException_WhenMonthlyFeeOrPerSaleFeeNegative(
        decimal monthlyFee, decimal perSaleFee)
    {
        var regularFeePercent = 0.01f;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var plan = new SellingPlan("Business", monthlyFee,
                perSaleFee, regularFeePercent);
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void Constructor_ThrowsException_WhenRegularPerSaleFeePercentNotPositive(
        float regularFeePercent)
    {
        var monthlyFee = 40m;
        var perSaleFee = 0.99m;

        Assert.Throws<RegularPerSaleFeePercentNotPositiveException>(() =>
        {
            var plan = new SellingPlan("Business", monthlyFee,
                perSaleFee, regularFeePercent);
        });
    }

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

    [Fact]
    public void ChangeFees_ThrowsException_WhenMonthlyFeeAndPerSaleFeeAreZero()
    {
        decimal monthlyFee = 0m;
        decimal perSaleFee = 0m;
        var regularFeePercent = 0.02f;

        Assert.Throws<MonthlyAndPerSaleFeesEqualZeroException>(
            () => _existingPlan.ChangeFees(monthlyFee, perSaleFee, regularFeePercent));
    }

    [Theory]
    [InlineData(40, -0.99)]
    [InlineData(-40, 0.99)]
    [InlineData(-40, -0.99)]
    public void ChangeFees_ThrowsException_WhenMonthlyFeeAndPerSaleFeeNotPositive(
        decimal monthlyFee, decimal perSaleFee)
    {
        var regularFeePercent = 0.02f;

        Assert.Throws<ArgumentOutOfRangeException>(
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
