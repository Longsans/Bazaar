using Moq;

namespace ContractingTests.UnitTests;

public class UpdatePartnerEmailAddressServiceUnitTests
{
    private readonly Mock<IPartnerRepository> _mockPartnerRepo;
    private readonly Partner _testPartner;

    public UpdatePartnerEmailAddressServiceUnitTests()
    {
        var mock = new Mock<IPartnerRepository>();
        _testPartner = new(
            1, "PNER-1", "Test", "Test",
            "test@testmail.com", "0901234567",
            new DateTime(1989, 11, 11), Gender.Male);

        mock.Setup(x => x.GetWithContractsByExternalId(It.IsAny<string>()))
            .Returns(_testPartner);

        _mockPartnerRepo = mock;
    }

    [Fact]
    public void UpdatePartnerEmailAddress_Succeeds_WhenEmailAddressNotExistYet()
    {
        // arrange
        _mockPartnerRepo.Setup(
            x => x.GetWithContractsByEmailAddress(It.IsAny<string>()))
            .Returns(() => null);

        var service = new UpdatePartnerEmailAddressService(_mockPartnerRepo.Object);

        // act
        var result = service.UpdatePartnerEmailAddress(
            _testPartner.ExternalId, "newmail@testmail.com");

        // assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void UpdatePartnerEmailAddress_Succeeds_WhenEmailAddressDoesNotChange()
    {
        // arrange
        _mockPartnerRepo.Setup(
            x => x.GetWithContractsByEmailAddress(_testPartner.EmailAddress))
            .Returns(_testPartner);

        var service = new UpdatePartnerEmailAddressService(_mockPartnerRepo.Object);

        // act
        var result = service.UpdatePartnerEmailAddress(
            _testPartner.ExternalId, _testPartner.EmailAddress);

        // assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void UpdatePartnerEmailAddress_ReturnsNotFound_WhenPartnerNotFound()
    {
        // arrange
        _mockPartnerRepo.Setup(
            x => x.GetWithContractsByExternalId(It.IsAny<string>()))
            .Returns(() => null);

        _mockPartnerRepo.Setup(
            x => x.GetWithContractsByEmailAddress(_testPartner.EmailAddress))
            .Returns(_testPartner);

        var service = new UpdatePartnerEmailAddressService(_mockPartnerRepo.Object);

        // act
        var result = service.UpdatePartnerEmailAddress(
            _testPartner.ExternalId, _testPartner.EmailAddress);

        // assert
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void UpdatePartnerEmailAddress_ReturnsConflict_WhenEmailAddressAlreadyExists()
    {
        // arrange
        var emailAddressOwner = _testPartner.Clone();

        _mockPartnerRepo.Setup(
            x => x.GetWithContractsByEmailAddress(It.IsAny<string>()))
            .Returns(emailAddressOwner);

        var service = new UpdatePartnerEmailAddressService(_mockPartnerRepo.Object);

        // act
        var result = service.UpdatePartnerEmailAddress(
            _testPartner.ExternalId, _testPartner.EmailAddress);

        // assert
        Assert.Equal(ResultStatus.Conflict, result.Status);
    }
}
