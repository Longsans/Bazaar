using ContractingTests.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ContractingTests.RepositoryTests;

public class ContractRepositoryTests
{
    private readonly IContractRepository _contractRepo;
    private readonly ContractingDbContext _dbContext;

    public ContractRepositoryTests(IContractRepository contractRepo, ContractingDbContext dbContext)
    {
        _contractRepo = contractRepo;
        _dbContext = dbContext;
        _dbContext.Reseed();
    }

    [Fact]
    public void GetById_ReturnsContract_WhenFound()
    {
        // arrange
        int id = 1;

        // act
        var contract = _contractRepo.GetById(id);

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
        var contract = _contractRepo.GetById(id);

        // assert
        Assert.Null(contract);
    }

    [Fact]
    public void GetByPartnerId_ReturnsContracts_WhenFound()
    {
        // arrange
        string partnerId = "PNER-2";

        // act
        var contracts = _contractRepo.GetByPartnerId(partnerId);

        // assert
        Assert.NotEmpty(contracts);
    }

    [Fact]
    public void GetByPartnerId_ReturnsEmpty_WhenNotFound()
    {
        // arrange
        string partnerId = "PNER-1";

        // act
        var contracts = _contractRepo.GetByPartnerId(partnerId);

        // assert
        Assert.Empty(contracts);
    }

    [Fact]
    public void CreateFixedPeriod_ReturnsSuccessResult_WhenValid()
    {
        // arrange
        var contract = ValidContract;

        // act
        var createResult = _contractRepo.CreateFixedPeriod(contract);

        // assert
        AssertCreated(createResult as ContractRepositoryResult, contract, false);
    }

    [Fact]
    public void CreateFixedPeriod_ReturnsError_WhenEndDateBeforeCurrentDate()
    {
        // arrange
        var contract = ValidContract.WithPastEndDate();

        // act
        var createResult = _contractRepo.CreateFixedPeriod(contract);

        // assert
        Assert.IsType<ContractEndDateBeforeCurrentDate>(createResult);
        AssertNotCreated(contract);
    }

    [Fact]
    public void CreateFixedPeriod_ReturnsError_WhenPartnerNotFound()
    {
        // arrange
        var contract = ValidContract.WithInvalidPartner();

        // act
        var createResult = _contractRepo.CreateFixedPeriod(contract);

        // assert
        Assert.IsType<PartnerNotFoundError>(createResult);
        AssertNotCreated(contract);
    }

    [Fact]
    public void CreateFixedPeriod_ReturnsError_WhenSellingPlanNotFound()
    {
        // arrange
        var contract = ValidContract.WithInvalidSellingPlan();

        // act
        var createResult = _contractRepo.CreateFixedPeriod(contract);

        // assert
        Assert.IsType<SellingPlanNotFoundError>(createResult);
        AssertNotCreated(contract);
    }

    [Fact]
    public void CreateFixedPeriod_ReturnsError_WhenPartnerUnderContract()
    {
        // arrange
        var contract = ValidContract.WithAlreadyContractedPartner();

        // act
        var createResult = _contractRepo.CreateFixedPeriod(contract);

        // assert
        Assert.IsType<PartnerUnderContractError>(createResult);
        AssertNotCreated(contract);
    }

    [Fact]
    public void CreateIndefinite_ReturnsSuccess_WhenValid()
    {
        // arrange
        var contract = ValidContract;

        // act
        var createResult = _contractRepo.CreateIndefinite(contract);

        // assert
        AssertCreated(createResult as ContractRepositoryResult, contract, true);
    }

    [Fact]
    public void CreateIndefinite_ReturnsError_WhenPartnerNotFound()
    {
        // arrange
        var contract = ValidContract.WithInvalidPartner();

        // act
        var createResult = _contractRepo.CreateIndefinite(contract);

        // assert
        Assert.IsType<PartnerNotFoundError>(createResult);
        AssertNotCreated(contract);
    }

    [Fact]
    public void CreateIndefinite_ReturnsError_WhenSellingPlanNotFound()
    {
        // arrange
        var contract = ValidContract.WithInvalidSellingPlan();

        // act
        var createResult = _contractRepo.CreateIndefinite(contract);

        // assert
        Assert.IsType<SellingPlanNotFoundError>(createResult);
        AssertNotCreated(contract);
    }

    [Fact]
    public void CreateIndefinite_ReturnsError_WhenPartnerUnderContract()
    {
        // arrange
        var contract = ValidContract.WithAlreadyContractedPartner();

        // act
        var createResult = _contractRepo.CreateIndefinite(contract);

        // assert
        Assert.IsType<PartnerUnderContractError>(createResult);
        AssertNotCreated(contract);
    }

    [Fact]
    public void EndPartnerIndefiniteContract_ReturnsSuccess_WhenValid()
    {
        // arrange
        int partnerId = 2;

        // act
        var endResult = _contractRepo.EndIndefiniteContract(partnerId);

        // assert
        var successResult = Assert.IsType<ContractSuccessResult>(endResult);
        Assert.NotNull(successResult.Contract);
        Assert.Equal(partnerId, successResult.Contract.PartnerId);
        Assert.Equal(DateTime.Now.Date, successResult.Contract.EndDate);
        AssertCurrentContract(partnerId, true);
    }

    [Fact]
    public void EndPartnerIndefiniteContract_ReturnsError_WhenPartnerNotFound()
    {
        // arrange
        int partnerId = 0;

        // act
        var endResult = _contractRepo.EndIndefiniteContract(partnerId);

        // assert
        Assert.IsType<PartnerNotFoundError>(endResult);
    }

    [Fact]
    public void EndPartnerIndefiniteContract_ReturnsError_WhenPartnerNotUnderContract()
    {
        // arrange
        int partnerId = 1;

        // act
        var endResult = _contractRepo.EndIndefiniteContract(partnerId);

        // assert
        Assert.IsType<ContractNotFoundError>(endResult);
    }

    [Fact]
    public void EndPartnerIndefiniteContract_ReturnsError_WhenContractNotIndefinite()
    {
        // arrange
        int partnerId = 3;

        // act
        var endResult = _contractRepo.EndIndefiniteContract(partnerId);

        // assert
        Assert.IsType<ContractNotIndefiniteError>(endResult);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    public void ExtendFixedPeriod_ReturnsSuccess_WhenValid(int contractId)
    {
        // act
        var extendResult = _contractRepo.ExtendFixedPeriodContract(
            contractId, ValidExtendedEndDate);

        // assert
        var successResult = Assert.IsType<ContractSuccessResult>(extendResult);
        Assert.NotNull(successResult.Contract);
        Assert.Equal(ValidExtendedEndDate, successResult.Contract.EndDate);

        var dbContract = _dbContext.Contracts.Find(contractId);
        Assert.Equal(ValidExtendedEndDate, dbContract!.EndDate);
    }

    [Theory]
    [MemberData(nameof(AllInvalidEndDates))]
    public void ExtendFixedPeriod_ReturnsError_WhenEndDateNotAfterOldEndDate(
        DateTime extendedEndDate)
    {
        // arrange
        var contractId = 2;
        var dbContract = _dbContext.Contracts.Find(contractId);
        var originalEndDate = dbContract.EndDate;

        // act
        var extendResult = _contractRepo.ExtendFixedPeriodContract(
            contractId, extendedEndDate);

        // assert
        Assert.IsType<EndDateNotAfterOldEndDateError>(extendResult);
        Assert.Equal(originalEndDate, dbContract.EndDate);
    }

    [Fact]
    public void ExtendFixedPeriod_ReturnsError_WhenContractNotFound()
    {
        // arrange
        var contractId = 0;

        // act
        var extendResult = _contractRepo.ExtendFixedPeriodContract(
            contractId, ValidExtendedEndDate);

        // assert
        Assert.IsType<ContractNotFoundError>(extendResult);
    }

    [Fact]
    public void ExtendFixedPeriod_ReturnsError_WhenContractNotFixedPeriod()
    {
        // arrange
        var contractId = 1;
        var dbContract = _dbContext.Contracts.Find(contractId);

        // act
        var extendResult = _contractRepo.ExtendFixedPeriodContract(
            contractId, ValidExtendedEndDate);

        // assert
        Assert.IsType<ContractNotFixedPeriodError>(extendResult);
        Assert.Null(dbContract.EndDate);
    }

    [Fact]
    public void ExtendFixedPeriod_ReturnsError_WhenContractEnded()
    {
        // arrange
        var contractId = 3;
        var dbContract = _dbContext.Contracts.Find(contractId);
        var originalEndDate = dbContract.EndDate;

        // act
        var extendResult = _contractRepo.ExtendFixedPeriodContract(
            contractId, ValidExtendedEndDate);

        // assert
        Assert.IsType<ContractEndedError>(extendResult);
        Assert.Equal(originalEndDate, dbContract.EndDate);
    }

    #region Constants and helpers
    private static Contract ValidContract => new()
    {
        PartnerId = 1,
        SellingPlanId = 1,
        EndDate = DateTime.Now.Date + TimeSpan.FromDays(30)
    };

    private static DateTime ValidExtendedEndDate
        => DateTime.Now.Date + TimeSpan.FromDays(60);

    public static IEnumerable<object[]> AllInvalidEndDates
        => new List<object[]>()
        {
            new object[] { DateTime.Now.Date },
            new object[] { DateTime.Now.Date + TimeSpan.FromDays(14) }
        };

    private void AssertCreated(
        ContractRepositoryResult result, Contract contractToCreate, bool indefinite)
    {
        var successResult = Assert.IsType<ContractSuccessResult>(result);
        Assert.NotNull(successResult.Contract);
        Assert.NotEqual(0, successResult.Contract.Id);
        Assert.Equal(contractToCreate.PartnerId, successResult.Contract.PartnerId);
        Assert.Equal(contractToCreate.SellingPlanId, successResult.Contract.SellingPlanId);
        Assert.Equal(DateTime.Now.Date, successResult.Contract.StartDate);

        if (indefinite)
        {
            Assert.Null(successResult.Contract.EndDate);
        }
        else
        {
            Assert.Equal(contractToCreate.EndDate, successResult.Contract.EndDate);
        }

        var dbContract = _dbContext.Contracts
            .AsEnumerable()
            .Single(c => IsAddedContract(c, contractToCreate));

        Assert.Equal(DateTime.Now.Date, dbContract.StartDate);

        if (indefinite)
        {
            Assert.Null(dbContract.EndDate);
        }
        else
        {
            Assert.Equal(contractToCreate.EndDate, dbContract.EndDate);
        }
    }

    private void AssertNotCreated(Contract contract)
    {
        var created = _dbContext.Contracts
            .AsEnumerable()
            .Any(c => IsAddedContract(c, contract));

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

    private static Func<Contract, Contract, bool> IsAddedContract =>
        (c, contractToAdd) => c.PartnerId == contractToAdd.PartnerId
                && c.SellingPlanId == contractToAdd.SellingPlanId
                && c.StartDate == DateTime.Now.Date;
    #endregion
}
