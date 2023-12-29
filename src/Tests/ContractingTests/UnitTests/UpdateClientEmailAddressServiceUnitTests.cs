namespace ContractingTests.UnitTests;

public class UpdateClientEmailAddressServiceUnitTests
{
    private readonly UpdateClientEmailAddressService _service;
    private readonly Mock<IRepository<Client>> _mockClientRepo;
    private readonly Client _testClient;

    private static void SetUpRepoGetById(
        Mock<IRepository<Client>> repoMock, Client? singleOrDefaultResult)
    {
        repoMock.Setup(x => x.SingleOrDefaultAsync(
                It.IsAny<ClientByExternalIdSpec>(),
                CancellationToken.None))
            .Returns(Task.FromResult(singleOrDefaultResult));
    }

    private static void SetUpRepoGetByEmailAddress(
        Mock<IRepository<Client>> repoMock, Client? singleOrDefaultResult)
    {
        repoMock.Setup(x => x.SingleOrDefaultAsync(
                It.IsAny<ClientByEmailAddressSpec>(),
                CancellationToken.None))
            .Returns(Task.FromResult(singleOrDefaultResult));
    }

    public UpdateClientEmailAddressServiceUnitTests()
    {
        var mock = new Mock<IRepository<Client>>();
        var sellingPlan = new SellingPlan("Individual", 0m, 1.99m, 0.05f);
        _testClient = new("Test", "Test",
            "test@testmail.com", "0901234567",
            new DateTime(1989, 11, 11), Gender.Male, sellingPlan.Id);
        _mockClientRepo = mock;
        _service = new(_mockClientRepo.Object);
    }

    [Fact]
    public async Task UpdateClientEmailAddress_Succeeds_WhenEmailAddressNotExistYet()
    {
        // arrange
        SetUpRepoGetById(_mockClientRepo, _testClient);
        SetUpRepoGetByEmailAddress(_mockClientRepo, null);

        // act
        var result = await _service.UpdateClientEmailAddress(
            _testClient.ExternalId, "newmail@testmail.com");

        // assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateClientEmailAddress_Succeeds_WhenEmailAddressDoesNotChange()
    {
        // arrange
        SetUpRepoGetById(_mockClientRepo, _testClient);
        SetUpRepoGetByEmailAddress(_mockClientRepo, _testClient);

        // act
        var result = await _service.UpdateClientEmailAddress(
            _testClient.ExternalId, _testClient.EmailAddress);

        // assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateClientEmailAddress_ReturnsNotFound_WhenClientNotFound()
    {
        // arrange
        SetUpRepoGetById(_mockClientRepo, null);

        // act
        var result = await _service.UpdateClientEmailAddress(
            _testClient.ExternalId, _testClient.EmailAddress);

        // assert
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task UpdateClientEmailAddress_ReturnsConflict_WhenEmailAddressAlreadyExists()
    {
        // arrange
        var emailAddressOwner = _testClient.WithDifferentId(2, "CLNT-2");
        SetUpRepoGetById(_mockClientRepo, _testClient);
        SetUpRepoGetByEmailAddress(_mockClientRepo, emailAddressOwner);

        // act
        var result = await _service.UpdateClientEmailAddress(
            _testClient.ExternalId, _testClient.EmailAddress);

        // assert
        Assert.Equal(ResultStatus.Conflict, result.Status);
    }
}
