using Moq;

namespace ContractingTests.UnitTests;

public class UpdateClientEmailAddressServiceUnitTests
{
    private readonly Mock<IClientRepository> _mockClientRepo;
    private readonly Client _testClient;

    public UpdateClientEmailAddressServiceUnitTests()
    {
        var mock = new Mock<IClientRepository>();
        _testClient = new(
            1, "CLNT-1", "Test", "Test",
            "test@testmail.com", "0901234567",
            new DateTime(1989, 11, 11), Gender.Male);

        mock.Setup(x => x.GetWithContractsByExternalId(It.IsAny<string>()))
            .Returns(_testClient);

        _mockClientRepo = mock;
    }

    [Fact]
    public void UpdateClientEmailAddress_Succeeds_WhenEmailAddressNotExistYet()
    {
        // arrange
        _mockClientRepo.Setup(
            x => x.GetWithContractsByEmailAddress(It.IsAny<string>()))
            .Returns(() => null);

        var service = new UpdateClientEmailAddressService(_mockClientRepo.Object);

        // act
        var result = service.UpdateClientEmailAddress(
            _testClient.ExternalId, "newmail@testmail.com");

        // assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void UpdateClientEmailAddress_Succeeds_WhenEmailAddressDoesNotChange()
    {
        // arrange
        _mockClientRepo.Setup(
            x => x.GetWithContractsByEmailAddress(_testClient.EmailAddress))
            .Returns(_testClient);

        var service = new UpdateClientEmailAddressService(_mockClientRepo.Object);

        // act
        var result = service.UpdateClientEmailAddress(
            _testClient.ExternalId, _testClient.EmailAddress);

        // assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void UpdateClientEmailAddress_ReturnsNotFound_WhenClientNotFound()
    {
        // arrange
        _mockClientRepo.Setup(
            x => x.GetWithContractsByExternalId(It.IsAny<string>()))
            .Returns(() => null);

        _mockClientRepo.Setup(
            x => x.GetWithContractsByEmailAddress(_testClient.EmailAddress))
            .Returns(_testClient);

        var service = new UpdateClientEmailAddressService(_mockClientRepo.Object);

        // act
        var result = service.UpdateClientEmailAddress(
            _testClient.ExternalId, _testClient.EmailAddress);

        // assert
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void UpdateClientEmailAddress_ReturnsConflict_WhenEmailAddressAlreadyExists()
    {
        // arrange
        var emailAddressOwner = _testClient.WithDifferentId(2, "CLNT-2");

        _mockClientRepo.Setup(
            x => x.GetWithContractsByEmailAddress(It.IsAny<string>()))
            .Returns(emailAddressOwner);

        var service = new UpdateClientEmailAddressService(_mockClientRepo.Object);

        // act
        var result = service.UpdateClientEmailAddress(
            _testClient.ExternalId, _testClient.EmailAddress);

        // assert
        Assert.Equal(ResultStatus.Conflict, result.Status);
    }
}
