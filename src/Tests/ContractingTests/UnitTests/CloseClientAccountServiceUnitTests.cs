namespace ContractingTests.UnitTests;

public class CloseClientAccountServiceUnitTests
{
    private readonly Client _testClient;

    private readonly CloseClientAccountService _service;
    private readonly EventBusTestDouble _testEventBus;
    private readonly Mock<IClientRepository> _repoMock;

    public CloseClientAccountServiceUnitTests(EventBusTestDouble testEventBus)
    {
        _testEventBus = testEventBus;
        _testClient = new("Test", "Test", "test@testmail.com", "0123456789",
            new DateTime(1979, 11, 11), Gender.Male, 1);

        _repoMock = new Mock<IClientRepository>();
        _repoMock.Setup(x => x.GetWithContractsAndPlanByExternalId(
            It.IsAny<string>()))
            .Returns(_testClient);

        _service = new(_repoMock.Object, _testEventBus);
    }

    [Fact]
    public void CloseAccount_ReturnsSuccess_WhenValid()
    {
        var result = _service.CloseAccount("clientid");

        Assert.True(result.IsSuccess);
        Assert.True(_testClient.IsAccountClosed);
    }

    [Fact]
    public void CloseAccount_ReturnsNotFound_WhenClientNotFound()
    {
        _repoMock.Setup(x => x.GetWithContractsAndPlanByExternalId(
            It.IsAny<string>()))
            .Returns(() => null);

        var result = _service.CloseAccount("clientid");

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void CloseAccount_ReturnsNotFound_WhenClientAccountIsClosed()
    {
        typeof(Client).GetProperty(nameof(_testClient.IsAccountClosed))!
            .SetValue(_testClient, true);

        var result = _service.CloseAccount("clientid");

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }
}
