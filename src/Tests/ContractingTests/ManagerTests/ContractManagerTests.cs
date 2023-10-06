using Microsoft.EntityFrameworkCore;

namespace ContractingTests.ManagerTests;

public class ContractManagerTests
{
    private readonly ContractManager _contractManager;
    private readonly ContractingDbContext _dbContext;

    public ContractManagerTests(ContractManager contractRepo, ContractingDbContext dbContext)
    {
        _contractManager = contractRepo;
        _dbContext = dbContext;
        _dbContext.Reseed();
    }

    [Fact]
    public void GetById_ReturnsContract_WhenFound()
    {
        // arrange
        int id = 1;

        // act
        var contract = _contractManager.GetById(id);

        // assert
        Assert.NotNull(contract);
        Assert.Equal(id, contract.Id);
        Assert.NotNull(contract.Partner);
        Assert.NotNull(contract.SellingPlan);
    }

    [Fact]
    public void GetById_ReturnsNull_WhenNotFound()
    {
        // arrange
        int id = 0;

        // act
        var contract = _contractManager.GetById(id);

        // assert
        Assert.Null(contract);
    }

    [Fact]
    public void GetByPartnerId_ReturnsContracts_WhenFound()
    {
        // arrange
        string partnerId = "PNER-2";

        // act
        var contracts = _contractManager.GetByPartnerExternalId(partnerId);

        // assert
        Assert.NotEmpty(contracts);
    }

    [Fact]
    public void GetByPartnerId_ReturnsEmpty_WhenNotFound()
    {
        // arrange
        string partnerId = "PNER-1";

        // act
        var contracts = _contractManager.GetByPartnerExternalId(partnerId);

        // assert
        Assert.Empty(contracts);
    }

    [Fact]
    public void SignFixedPeriod_ReturnsSuccessResult_WhenValid()
    {
        // act
        var createResult = _contractManager.SignPartnerForFixedPeriod(
                UnsignedPartnerExternalId, ValidSellingPlanId, ValidExtendedEndDate);

        // assert
        AssertCreated(createResult as ContractManagementResult, ValidExtendedEndDate);
    }

    [Fact]
    public void SignFixedPeriod_ReturnsError_WhenEndDateBeforeCurrentDate()
    {
        // arrange
        var endDate = DateTime.Now.Date - TimeSpan.FromDays(1);

        // act
        var createResult = _contractManager.SignPartnerForFixedPeriod(
            UnsignedPartnerExternalId, ValidSellingPlanId, endDate);

        // assert
        Assert.IsType<ContractEndDateBeforeCurrentDate>(createResult);
        AssertNotCreated(UnsignedPartnerId, ValidSellingPlanId);
    }

    [Fact]
    public void SignFixedPeriod_ReturnsError_WhenPartnerNotFound()
    {
        // arrange
        int partnerId = 0;

        // act
        var createResult = _contractManager.SignPartnerForFixedPeriod(
            ToExternalId(partnerId), ValidSellingPlanId, ValidExtendedEndDate);

        // assert
        Assert.IsType<PartnerNotFoundError>(createResult);
        AssertNotCreated(partnerId, ValidSellingPlanId);
    }

    [Fact]
    public void SignFixedPeriod_ReturnsError_WhenSellingPlanNotFound()
    {
        // arrange
        int sellingPlanId = 0;

        // act
        var createResult = _contractManager.SignPartnerForFixedPeriod(
            UnsignedPartnerExternalId, sellingPlanId, ValidExtendedEndDate);

        // assert
        Assert.IsType<ContractSellingPlanNotFoundError>(createResult);
        AssertNotCreated(UnsignedPartnerId, sellingPlanId);
    }

    [Fact]
    public void SignFixedPeriod_ReturnsError_WhenPartnerUnderContract()
    {
        // arrange
        int partnerId = 2;

        // act
        var createResult = _contractManager.SignPartnerForFixedPeriod(
            ToExternalId(partnerId), ValidSellingPlanId, ValidExtendedEndDate);

        // assert
        Assert.IsType<PartnerUnderContractError>(createResult);
        AssertNotCreated(partnerId, ValidSellingPlanId);
    }

    [Fact]
    public void SignPartnerIndefinitely_ReturnsSuccess_WhenValid()
    {
        // act
        var createResult = _contractManager.SignPartnerIndefinitely(
            UnsignedPartnerExternalId, ValidSellingPlanId);

        // assert
        AssertCreated(createResult as ContractManagementResult);
    }

    [Fact]
    public void SignPartnerIndefinitely_ReturnsError_WhenPartnerNotFound()
    {
        // arrange
        int partnerId = 0;

        // act
        var createResult = _contractManager.SignPartnerIndefinitely(
            ToExternalId(partnerId), ValidSellingPlanId);

        // assert
        Assert.IsType<PartnerNotFoundError>(createResult);
        AssertNotCreated(partnerId, ValidSellingPlanId);
    }

    [Fact]
    public void SignPartnerIndefinitely_ReturnsError_WhenSellingPlanNotFound()
    {
        // arrange
        int sellingPlanId = 0;

        // act
        var createResult = _contractManager.SignPartnerIndefinitely(
            UnsignedPartnerExternalId, sellingPlanId);

        // assert
        Assert.IsType<ContractSellingPlanNotFoundError>(createResult);
        AssertNotCreated(UnsignedPartnerId, sellingPlanId);
    }

    [Fact]
    public void SignPartnerIndefinitely_ReturnsError_WhenPartnerUnderContract()
    {
        // act
        var createResult = _contractManager.SignPartnerIndefinitely(
            FpPartnerExternalId, ValidSellingPlanId);

        // assert
        Assert.IsType<PartnerUnderContractError>(createResult);
        AssertNotCreated(FpPartnerId, ValidSellingPlanId);
    }

    [Fact]
    public void EndPartnerIndefiniteContract_ReturnsSuccess_WhenValid()
    {
        // act
        var endResult = _contractManager
            .EndCurrentIndefiniteContractWithPartner(IndefPartnerExternalId);

        // assert
        var successResult = Assert.IsType<ContractSuccessResult>(endResult);
        Assert.NotNull(successResult.Contract);
        Assert.Equal(IndefPartnerId, successResult.Contract.PartnerId);
        Assert.Equal(DateTime.Now.Date, successResult.Contract.EndDate);
        AssertCurrentContract(IndefPartnerId, true);
    }

    [Fact]
    public void EndPartnerIndefiniteContract_ReturnsError_WhenPartnerNotFound()
    {
        // arrange
        int partnerId = 0;

        // act
        var endResult = _contractManager
            .EndCurrentIndefiniteContractWithPartner(ToExternalId(partnerId));

        // assert
        Assert.IsType<PartnerNotFoundError>(endResult);
    }

    [Fact]
    public void EndPartnerIndefiniteContract_ReturnsError_WhenPartnerNotUnderContract()
    {
        // act
        var endResult = _contractManager
            .EndCurrentIndefiniteContractWithPartner(UnsignedPartnerExternalId);

        // assert
        Assert.IsType<ContractNotFoundError>(endResult);
    }

    [Fact]
    public void EndPartnerIndefiniteContract_ReturnsError_WhenContractNotIndefinite()
    {
        // act
        var endResult = _contractManager
            .EndCurrentIndefiniteContractWithPartner(FpPartnerExternalId);

        // assert
        Assert.IsType<ContractNotIndefiniteError>(endResult);
    }

    [Theory]
    [InlineData(FpPartnerId)]
    [InlineData(FpEndTodayPartnerId)]
    public void ExtendFixedPeriod_ReturnsSuccess_WhenValid(int partnerId)
    {
        // act
        var extendResult = _contractManager
            .ExtendCurrentFixedPeriodContractWithPartner(
                ToExternalId(partnerId), ValidExtendedEndDate);

        // assert
        var successResult = Assert.IsType<ContractSuccessResult>(extendResult);
        Assert.NotNull(successResult.Contract);
        Assert.Equal(ValidExtendedEndDate, successResult.Contract.EndDate);

        var dbContract = _dbContext.Partners
            .Include(p => p.Contracts)
            .Single(p => p.Id == partnerId)
            .CurrentContract;
        Assert.Equal(ValidExtendedEndDate, dbContract.EndDate);
    }

    [Theory]
    [MemberData(nameof(AllInvalidEndDates))]
    public void ExtendFixedPeriod_ReturnsError_WhenEndDateNotAfterOldEndDate(
        DateTime extendedEndDate)
    {
        // arrange
        var dbContract = _dbContext.Partners
            .Include(p => p.Contracts)
            .Single(p => p.Id == FpPartnerId)
            .CurrentContract;
        var originalEndDate = dbContract.EndDate;

        // act
        var extendResult = _contractManager
            .ExtendCurrentFixedPeriodContractWithPartner(
                FpPartnerExternalId, extendedEndDate);

        // assert
        Assert.IsType<EndDateNotAfterOldEndDateError>(extendResult);
        Assert.Equal(originalEndDate, dbContract.EndDate);
    }

    [Fact]
    public void ExtendFixedPeriod_ReturnsError_WhenContractNotFound()
    {
        // act
        var extendResult = _contractManager
            .ExtendCurrentFixedPeriodContractWithPartner(
                UnsignedPartnerExternalId, ValidExtendedEndDate);

        // assert
        Assert.IsType<ContractNotFoundError>(extendResult);
    }

    [Fact]
    public void ExtendFixedPeriod_ReturnsError_WhenContractNotFixedPeriod()
    {
        // arrange
        var dbContract = _dbContext.Partners
            .Include(p => p.Contracts)
            .Single(p => p.Id == IndefPartnerId)
            .CurrentContract;

        // act
        var extendResult = _contractManager
            .ExtendCurrentFixedPeriodContractWithPartner(
                IndefPartnerExternalId, ValidExtendedEndDate);

        // assert
        Assert.IsType<ContractNotFixedPeriodError>(extendResult);
        Assert.Null(dbContract.EndDate);
    }

    #region Constants and helpers

    private const int UnsignedPartnerId = 1;
    private const string UnsignedPartnerExternalId = "PNER-1";

    private const int IndefPartnerId = 2;
    private const string IndefPartnerExternalId = "PNER-2";

    private const int FpPartnerId = 3;
    private const string FpPartnerExternalId = "PNER-3";

    private const int FpEndTodayPartnerId = 4;

    private const int ValidSellingPlanId = 1;
    private static DateTime ValidExtendedEndDate
        => DateTime.Now.Date + TimeSpan.FromDays(60);


    private static string ToExternalId(int id)
        => $"PNER-{id}";


    public static IEnumerable<object[]> AllInvalidEndDates
        => new List<object[]>()
        {
            new object[] { DateTime.Now.Date },
            new object[] { DateTime.Now.Date + TimeSpan.FromDays(14) }
        };

    private void AssertCreated(
        ContractManagementResult result, DateTime? endDate = null)
    {
        var successResult = Assert.IsType<ContractSuccessResult>(result);
        Assert.NotNull(successResult.Contract);
        Assert.NotEqual(0, successResult.Contract.Id);
        Assert.Equal(UnsignedPartnerId, successResult.Contract.PartnerId);
        Assert.Equal(ValidSellingPlanId, successResult.Contract.SellingPlanId);
        Assert.Equal(DateTime.Now.Date, successResult.Contract.StartDate);

        if (endDate is null)
        {
            Assert.Null(successResult.Contract.EndDate);
        }
        else
        {
            Assert.Equal(endDate, successResult.Contract.EndDate);
        }

        var dbContract = _dbContext.Contracts
            .AsEnumerable()
            .Single(c => IsAddedContract(c, UnsignedPartnerId, ValidSellingPlanId));

        Assert.Equal(DateTime.Now.Date, dbContract.StartDate);

        if (endDate is null)
        {
            Assert.Null(dbContract.EndDate);
        }
        else
        {
            Assert.Equal(endDate, dbContract.EndDate);
        }
    }

    private void AssertNotCreated(int partnerId, int sellingPlanId)
    {
        var created = _dbContext.Contracts
            .AsEnumerable()
            .Any(c => IsAddedContract(c, partnerId, sellingPlanId));

        Assert.False(created);
    }

    private void AssertCurrentContract(int partnerId, bool expectedEnded)
    {
        var partner = _dbContext.Partners
            .Include(p => p.Contracts)
            .Single(p => p.Id == partnerId);

        var createdContract = partner.Contracts
            .Single(c =>
                partner.Contracts.All(c2 =>
                    c2.StartDate <= c.StartDate));

        if (expectedEnded)
        {
            Assert.NotNull(createdContract.EndDate);
        }
        else
        {
            Assert.Null(createdContract.EndDate);
        }
    }

    private static Func<Contract, int, int, bool> IsAddedContract =>
        (c, partnerId, sellingPlanId) => c.PartnerId == partnerId
                && c.SellingPlanId == sellingPlanId
                && c.StartDate == DateTime.Now.Date;

    #endregion
}
