using Microsoft.EntityFrameworkCore;

namespace ContractingTests.RepositoryTests;

public class ContractRepositoryTests
{
    private readonly IContractRepository _repo;
    private readonly ContractingDbContext _dbContext;

    public ContractRepositoryTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ContractingDbContext>();
        optionsBuilder.UseInMemoryDatabase("ContractRepoTests");

        _dbContext = new ContractingDbContext(optionsBuilder.Options);
        _repo = new ContractRepository(_dbContext);

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        _dbContext.Partners.Add(NewPartner);
        _dbContext.SellingPlans.Add(NewPlan);
        _dbContext.SaveChanges();

        _dbContext.Contracts.Add(NewFpContract);
        _dbContext.SaveChanges();
    }

    [Fact]
    public void GetById_ReturnsContract_WhenFound()
    {
        var existing = ExistingFpContract;

        var contract = _repo.GetById(existing.Id);

        AssertGet(existing, contract);
    }

    [Fact]
    public void GetById_ReturnsNull_WhenNotFound()
    {
        int id = 0;

        var contract = _repo.GetById(id);

        Assert.Null(contract);
    }

    [Fact]
    public void GetByPartnerExternalId_ReturnsContract_WhenFound()
    {
        var partnerExternalId = ExistingPartner.ExternalId;

        var contracts = _repo.GetByPartnerExternalId(partnerExternalId);

        Assert.NotEmpty(contracts);
        foreach (var contract in contracts)
        {
            AssertGet(ExistingFpContract, contract);
        }
    }

    [Fact]
    public void GetByPartnerExternalId_ReturnsNull_WhenNotFound()
    {
        var partnerExternalId = "PNER-0";

        var contracts = _repo.GetByPartnerExternalId(partnerExternalId);

        Assert.Empty(contracts);
    }

    [Fact]
    public void Create_ReturnsContract()
    {
        (var repo, var mockCtx, var mockSet) = GetSutAndMocks();

        mockSet.Setup(x => x.Add(It.IsAny<Contract>()))
            .Verifiable();

        mockCtx.Setup(x => x.SaveChanges())
            .Verifiable();

        var contract = repo.Create(ExistingFpContract);

        Assert.NotNull(contract);
        mockSet.Verify();
        mockCtx.Verify();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void FindAndUpdateEndDate_ReturnsUpdatedContract_WhenFoundAndValidEndDate(
        int daysFromNow)
    {
        (var repo, var mockCtx, var mockSet) = GetSutAndMocks();

        mockCtx.Setup(x => x.SaveChanges())
            .Verifiable();

        mockSet.Setup(x => x.Find(It.IsAny<int>()))
            .Returns(ExistingFpContract)
            .Verifiable();

        var endDate = DateTime.Now.Date + TimeSpan.FromDays(daysFromNow);

        var updatedContract = repo.FindAndUpdateEndDate(
            default, endDate);

        Assert.NotNull(updatedContract);
        Assert.Equal(updatedContract.EndDate, endDate);
        mockSet.Verify();
        mockCtx.Verify();
    }

    [Fact]
    public void FindAndUpdateEndDate_ReturnsNull_WhenContractNotFound()
    {
        (var repo, var mockCtx, var mockSet) = GetSutAndMocks();

        mockCtx.Setup(x => x.SaveChanges())
            .Verifiable();

        mockSet.Setup(x => x.Find(It.IsAny<int>()))
            .Returns(() => null)
            .Verifiable();

        var endDate = DateTime.Now.Date;

        var updatedContract = repo.FindAndUpdateEndDate(
            default, endDate);

        Assert.Null(updatedContract);
        mockSet.Verify();
        mockCtx.VerifyGet(x => x.Contracts);
        mockCtx.Verify(x => x.SaveChanges(), Times.Never);
    }

    [Fact]
    public void FindAndUpdateEndDate_ReturnsNull_WhenEndDateHasPassed()
    {
        (var repo, var mockCtx, var mockSet) = GetSutAndMocks();

        mockCtx.Setup(x => x.SaveChanges())
            .Verifiable();

        mockSet.Setup(x => x.Find(It.IsAny<int>()))
            .Returns(ExistingFpContract)
            .Verifiable();

        var endDate = DateTime.Now.Date - TimeSpan.FromDays(1);

        var updatedContract = repo.FindAndUpdateEndDate(
            default, endDate);

        Assert.Null(updatedContract);
        mockSet.Verify();
        mockCtx.VerifyGet(x => x.Contracts);
        mockCtx.Verify(x => x.SaveChanges(), Times.Never);
    }

    #region Constants and helpers

    private static readonly Contract ExistingFpContract
        = ContractExtensionsAndHelpers.ValidFpContract;

    private static readonly Partner ExistingPartner
        = PartnerExtensionsAndHelpers.ExistingPartner;

    private static readonly SellingPlan ExistingPlan
        = SellingPlanExtensionsAndHelpers.ExistingPlan;

    private static Contract NewFpContract
    {
        get
        {
            var contract = ExistingFpContract.Clone();
            contract.Id = 0;
            return contract;
        }
    }

    private static Partner NewPartner
    {
        get
        {
            var partner = ExistingPartner.Clone();
            partner.Id = 0;
            return partner;
        }
    }

    private static SellingPlan NewPlan
    {
        get
        {
            var plan = ExistingPlan.Clone();
            plan.Id = 0;
            return plan;
        }
    }

    private void AssertGet(Contract existing, Contract? queried)
    {
        Assert.NotNull(queried);
        Assert.Equal(existing.Id, queried.Id);
        Assert.Equal(existing.PartnerId, queried.PartnerId);
        Assert.Equal(existing.SellingPlanId, queried.SellingPlanId);
        Assert.Equal(existing.StartDate, queried.StartDate);
        Assert.Equal(existing.EndDate, queried.EndDate);
        Assert.NotNull(queried.Partner);
        Assert.NotNull(queried.SellingPlan);
    }

    private (
        ContractRepository contractRepo,
        Mock<ContractingDbContext> mockDbContext,
        Mock<DbSet<Contract>> mockDbSet) GetSutAndMocks()
    {
        var mockContractDbSet = new Mock<DbSet<Contract>>();
        var mockDbContext = new Mock<ContractingDbContext>();

        mockDbContext
            .Setup(x => x.Contracts)
            .Returns(mockContractDbSet.Object)
            .Verifiable();

        return (
            new ContractRepository(mockDbContext.Object),
            mockDbContext,
            mockContractDbSet);
    }
    #endregion
}
