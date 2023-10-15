namespace ContractingTests.ManagerTests;

public class SellingPlanManagerTests
{
    [Fact]
    public void GetById_ReturnsPlan_WhenFound()
    {
        // arrange
        (var planMgr, var mockPlanRepo) = GetSutAndMocks();
        int id = ExistingPlan.Id;

        mockPlanRepo
            .Setup(x => x.GetById(id))
            .Returns(ExistingPlan)
            .Verifiable();

        // act
        var plan = planMgr.GetById(id);

        // assert
        Assert.NotNull(plan);
        Assert.Equal(id, plan.Id);
        Assert.Equal(plan, ExistingPlan);
        mockPlanRepo.Verify(x => x.GetById(id), Times.Once);
    }

    [Fact]
    public void GetById_ReturnsNull_WhenNotFound()
    {
        // arrange
        (var planMgr, var mockPlanRepo) = GetSutAndMocks();
        int id = 0;

        mockPlanRepo
            .Setup(x => x.GetById(id))
            .Returns(() => null)
            .Verifiable();

        // act
        var plan = planMgr.GetById(id);

        // assert
        Assert.Null(plan);
        mockPlanRepo.Verify(x => x.GetById(id), Times.Once);
    }

    [Fact]
    public void CreateSellingPlan_ReturnsSuccess_WhenValid()
    {
        // arrange
        (var planMgr, var mockPlanRepo) = GetSutAndMocks();
        var plan = ValidNewPlan;

        mockPlanRepo
            .Setup(x => x.Create(plan))
            .Returns(() =>
            {
                plan.Id = 2;
                return plan;
            })
            .Verifiable();

        // act
        var result = planMgr.CreateSellingPlan(plan);

        // assert
        var successResult = Assert.IsType<SellingPlanSuccessResult>(result);
        Assert.NotNull(successResult.Plan);
        Assert.Equal(plan, successResult.Plan);
        Assert.Equal(2, successResult.Plan.Id);

        mockPlanRepo.Verify(x => x.Create(plan), Times.Once);
    }

    [Theory]
    [InlineData(-0.1, -0.1)]
    [InlineData(-0.1, 0.0)]
    [InlineData(0.0, -0.1)]
    [InlineData(0.0, 0.0)]
    public void CreateSellingPlan_ReturnsError_WhenPerSaleFeeAndMonthlyFeeNotPositive(
        decimal perSaleFee, decimal monthlyFee)
    {
        // arrange
        (var planMgr, var mockPlanRepo) = GetSutAndMocks();
        var plan = ValidNewPlan.Clone();
        plan.PerSaleFee = perSaleFee;
        plan.MonthlyFee = monthlyFee;

        mockPlanRepo
            .Setup(x => x.Create(plan))
            .Returns(() =>
            {
                plan.Id = 2;
                return plan;
            })
            .Verifiable();

        // act
        var result = planMgr.CreateSellingPlan(plan);

        // assert
        Assert.IsType<PerSaleFeeAndMonthlyFeeNotPositiveError>(result);
        mockPlanRepo.Verify(x => x.Create(plan), Times.Never);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(0.0)]
    public void CreateSellingPlan_ReturnsError_WhenRegularPerSaleFeeNotPositive(
        float regularFeePercent)
    {
        // arrange
        (var planMgr, var mockPlanRepo) = GetSutAndMocks();
        var plan = ValidNewPlan.Clone();
        plan.RegularPerSaleFeePercent = regularFeePercent;

        mockPlanRepo
            .Setup(x => x.Create(plan))
            .Returns(() =>
            {
                plan.Id = 2;
                return plan;
            })
            .Verifiable();

        // act
        var result = planMgr.CreateSellingPlan(plan);

        // assert
        Assert.IsType<RegularPerSaleFeeNotPositiveError>(result);
        mockPlanRepo.Verify(x => x.Create(plan), Times.Never);
    }

    [Fact]
    public void UpdateSellingPlan_ReturnsSuccess_WhenValid()
    {
        // arrange
        (var planMgr, var mockPlanRepo) = GetSutAndMocks();
        var plan = ExistingPlan.Clone();
        plan.MonthlyFee = 100m;
        plan.RegularPerSaleFeePercent = 0.15f;

        mockPlanRepo
            .Setup(x => x.FindAndUpdate(plan))
            .Returns(plan)
            .Verifiable();

        // act
        var result = planMgr.UpdateSellingPlan(plan);

        // assert
        var successResult = Assert.IsType<SellingPlanSuccessResult>(result);
        Assert.NotNull(successResult.Plan);
        Assert.Equal(plan, successResult.Plan);

        mockPlanRepo.Verify(x => x.FindAndUpdate(plan), Times.Once);
    }

    [Theory]
    [InlineData(-0.1, -0.1)]
    [InlineData(-0.1, 0.0)]
    [InlineData(0.0, -0.1)]
    [InlineData(0.0, 0.0)]
    public void UpdateSellingPlan_ReturnsError_WhenPerSaleFeeAndMonthlyFeeNotPositive(
        decimal perSaleFee, decimal monthlyFee)
    {
        // arrange
        (var planMgr, var mockPlanRepo) = GetSutAndMocks();
        var plan = ExistingPlan.Clone();
        plan.PerSaleFee = perSaleFee;
        plan.MonthlyFee = monthlyFee;

        mockPlanRepo
            .Setup(x => x.FindAndUpdate(plan))
            .Returns(plan)
            .Verifiable();

        // act
        var result = planMgr.UpdateSellingPlan(plan);

        // assert
        Assert.IsType<PerSaleFeeAndMonthlyFeeNotPositiveError>(result);
        mockPlanRepo.Verify(x => x.FindAndUpdate(plan), Times.Never);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(0.0)]
    public void UpdateSellingPlan_ReturnsError_WhenRegularPerSaleFeeNotPositive(
        float regularFeePercent)
    {
        // arrange
        (var planMgr, var mockPlanRepo) = GetSutAndMocks();
        var plan = ExistingPlan.Clone();
        plan.RegularPerSaleFeePercent = regularFeePercent;

        mockPlanRepo
            .Setup(x => x.FindAndUpdate(plan))
            .Returns(plan)
            .Verifiable();

        // act
        var result = planMgr.UpdateSellingPlan(plan);

        // assert
        Assert.IsType<RegularPerSaleFeeNotPositiveError>(result);
        mockPlanRepo.Verify(x => x.FindAndUpdate(plan), Times.Never);
    }

    [Fact]
    public void UpdateSellingPlan_ReturnsError_WhenSellingPlanNotFound()
    {
        // arrange
        (var planMgr, var mockPlanRepo) = GetSutAndMocks();
        var plan = ExistingPlan.Clone();
        plan.Id = 3;

        mockPlanRepo
            .Setup(x => x.FindAndUpdate(plan))
            .Returns(() => null)
            .Verifiable();

        // act
        var result = planMgr.UpdateSellingPlan(plan);

        // assert
        Assert.IsType<SellingPlanNotFoundError>(result);
        mockPlanRepo.Verify(x => x.FindAndUpdate(plan), Times.Once);
    }

    // Constants and helpers
    private static readonly SellingPlan ExistingPlan
        = SellingPlanExtensionsAndHelpers.ExistingPlan;

    private static readonly SellingPlan ValidNewPlan
        = SellingPlanExtensionsAndHelpers.ValidNewPlan;

    private static (
        SellingPlanManager,
        Mock<ISellingPlanRepository>) GetSutAndMocks()
    {
        var mockPlanRepo = new Mock<ISellingPlanRepository>();
        return (new SellingPlanManager(mockPlanRepo.Object), mockPlanRepo);
    }
}
