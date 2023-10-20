using Bazaar.Contracting.Infrastructure.Repositories;

namespace ContractingTests.IntegrationTests;

public class UpdateEmailAddressServiceIntegrationTests
{
    private readonly IPartnerRepository _partnerRepo;
    private readonly UpdatePartnerEmailAddressService _service;
    private readonly Partner _testPartner;

    public UpdateEmailAddressServiceIntegrationTests(ContractingDbContext dbContext)
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        _partnerRepo = new PartnerRepository(dbContext);
        _service = new UpdatePartnerEmailAddressService(_partnerRepo);

        _testPartner = new("Test", "Test",
            "test@testmail.com", "0901234567",
            new DateTime(1989, 11, 11), Gender.Male);
        _partnerRepo.Create(_testPartner);
    }

    [Fact]
    public void UpdatePartnerEmailAddress_Succeeds_WhenEmailAddressNotExistYet()
    {
        var newEmailAddress = "newmail@testmail.com";

        var result = _service.UpdatePartnerEmailAddress(
            _testPartner.ExternalId, newEmailAddress);

        Assert.True(result.IsSuccess);
        Assert.Equal(newEmailAddress, _testPartner.EmailAddress);

        var emailAddressOwner = _partnerRepo
            .GetWithContractsByEmailAddress(newEmailAddress);

        Assert.NotNull(emailAddressOwner);
        Assert.Equal(emailAddressOwner.Id, _testPartner.Id);
    }

    [Fact]
    public void UpdatePartnerEmailAddress_Succeeds_WhenEmailAddressDoesNotChange()
    {
        var originalEmailAddress = _testPartner.EmailAddress;

        var result = _service.UpdatePartnerEmailAddress(
            _testPartner.ExternalId, originalEmailAddress);

        Assert.True(result.IsSuccess);
        Assert.Equal(originalEmailAddress, _testPartner.EmailAddress);

        var emailAddressOwner = _partnerRepo
            .GetWithContractsByEmailAddress(originalEmailAddress);

        Assert.NotNull(emailAddressOwner);
        Assert.Equal(emailAddressOwner.Id, _testPartner.Id);
    }

    [Fact]
    public void UpdatePartnerEmailAddress_ReturnsNotFound_WhenPartnerNotFound()
    {
        var partnerExternalId = "PNER-1000";
        var newEmailAddress = "newmail@testmail.com";

        var result = _service.UpdatePartnerEmailAddress(
            partnerExternalId, newEmailAddress);

        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.NotEqual(newEmailAddress, _testPartner.EmailAddress);

        var newEmailAddressOwner = _partnerRepo
            .GetWithContractsByEmailAddress(newEmailAddress);

        Assert.Null(newEmailAddressOwner);
    }

    [Fact]
    public void UpdatePartnerEmailAddress_ReturnsConflict_WhenEmailAddressAlreadyExists()
    {
        // arrange
        var existingEmailAddress = "existing@testmail.com";
        var existingOwner = _testPartner.Clone()
            .WithEmailAddress(existingEmailAddress);

        _partnerRepo.Create(existingOwner);

        // act
        var result = _service.UpdatePartnerEmailAddress(
            _testPartner.ExternalId, existingEmailAddress);

        // assert
        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.NotEqual(existingEmailAddress, _testPartner.EmailAddress);

        var emailAddressOwner = _partnerRepo
            .GetWithContractsByEmailAddress(existingEmailAddress);

        Assert.NotNull(emailAddressOwner);
        Assert.NotEqual(emailAddressOwner.Id, _testPartner.Id);
    }
}
