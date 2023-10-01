using ContractingTests.Extensions;

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
    public void CreateFixedPeriod_ReturnsSuccessResult_WhenValid()
    {
        // arrange
        var contract = ValidContract;

        // act
        var createResult = _contractRepo.CreateFixedPeriod(contract);

        // assert
        Assert.IsType<ContractSuccessResult>(createResult);

        var createdContract = _dbContext.Contracts
            .AsEnumerable()
            .Single(c => IsAddedContract(c, contract));

        Assert.NotNull(createdContract.EndDate);
        Assert.Equal(ValidContract.EndDate, createdContract.EndDate);
    }

    [Fact]
    public void CreateFixedPeriod_ReturnsError_WhenContractEndDateBeforeCurrentDate()
    {
        // arrange
        var contract = ValidContract.WithPastEndDate();

        // act
        var createResult = _contractRepo.CreateFixedPeriod(contract);

        // assert
        Assert.IsType<ContractEndDateBeforeCurrentDate>(createResult);

        var added = _dbContext.Contracts
            .AsEnumerable()
            .Any(c => IsAddedContract(c, contract));
        Assert.False(added);
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

        var added = _dbContext.Contracts
            .AsEnumerable()
            .Any(c => IsAddedContract(c, contract));
        Assert.False(added);
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

        var added = _dbContext.Contracts
            .AsEnumerable()
            .Any(c => IsAddedContract(c, contract));
        Assert.False(added);
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

        var added = _dbContext.Contracts
            .AsEnumerable()
            .Any(c => IsAddedContract(c, contract));
        Assert.False(added);
    }

    //public static IEnumerable<object[]> FpContracts

    private static Contract ValidContract => new()
    {
        PartnerId = 1,
        SellingPlanId = 1,
        EndDate = DateTime.Now.Date + TimeSpan.FromDays(30)
    };

    private static Func<Contract, Contract, bool> IsAddedContract =>
        (c, added) => c.PartnerId == added.PartnerId
                && c.SellingPlanId == added.SellingPlanId
                && c.StartDate == DateTime.Now.Date;
}
