[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace ContractingTests.ControllerTests;

public class ContractControllerTests
{
    private readonly ContractController _contractCtrl;
    private readonly ContractingDbContext _dbContext;

    private const string VALID_PARTNER_ID = "PNER-1";
    private const int VALID_PLAN_ID = 1;
    private static readonly DateTime VALID_END_DATE = DateTime.Now.Date + TimeSpan.FromDays(30);

    public static IEnumerable<object[]> FpContractEndDates
        => new List<object[]>
        {
            new object[] { DateTime.Now.Date - TimeSpan.FromDays(1) },
        };

    public ContractControllerTests(ContractController contractController, ContractingDbContext dbContext)
    {
        _contractCtrl = contractController;
        _dbContext = dbContext;
        _dbContext.Reseed();
    }

    [Fact]
    public void SignUpForFpContract_ReturnsCreated_IfSuccess()
    {
        // arrange
        var command = new FixedPeriodContractCreateCommand()
        {
            SellingPlanId = VALID_PLAN_ID,
            EndDate = VALID_END_DATE
        };

        // act
        var result = _contractCtrl.SignUpForFixedPeriodContract(
            VALID_PARTNER_ID, command);

        // assert
        var objectResult = result.Result as ObjectResult;
        Assert.Equal(201, objectResult.StatusCode);

        var contract = (ContractQuery)objectResult.Value;
        Assert.Equal(VALID_PARTNER_ID, $"PNER-{contract.PartnerId}");
        Assert.Equal(VALID_PLAN_ID, contract.SellingPlanId);
        Assert.Equal(DateTime.Now.Date, contract.StartDate);
        Assert.Equal(VALID_END_DATE, contract.EndDate);
        Assert.NotEqual(0, contract.Id);
    }

    [Theory]
    [MemberData(nameof(FpContractEndDates))]
    public void SignUpForFpContract_ReturnsBadRequest_IfEndDateBeforeCurrentDate(
        DateTime endDate)
    {
        // arrange
        var command = new FixedPeriodContractCreateCommand()
        {
            SellingPlanId = VALID_PLAN_ID,
            EndDate = endDate
        };

        // act
        var result = _contractCtrl.SignUpForFixedPeriodContract(
            VALID_PARTNER_ID, command);

        // assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Theory]
    [InlineData("PNER-0", 0)]
    [InlineData("PNER-0", 1)]
    [InlineData("PNER-1", 0)]
    public void SignUpForFpContract_ReturnsNotFound_IfNoPartnerOrSellingPlanFound(
        string partnerExternalId, int sellingPlanId)
    {
        // arrange
        var command = new FixedPeriodContractCreateCommand()
        {
            SellingPlanId = sellingPlanId,
            EndDate = VALID_END_DATE
        };

        // act
        var result = _contractCtrl.SignUpForFixedPeriodContract(
            partnerExternalId, command);

        // assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public void SignUpForFpContract_ReturnsConflict_IfPartnerUnderContract()
    {
        // arrange
        var contractedPartnerId = "PNER-2";
        var command = new FixedPeriodContractCreateCommand()
        {
            SellingPlanId = VALID_PLAN_ID,
            EndDate = VALID_END_DATE
        };

        // act
        var result = _contractCtrl.SignUpForFixedPeriodContract(
            contractedPartnerId, command);

        // assert
        Assert.IsType<ConflictObjectResult>(result.Result);
    }
}