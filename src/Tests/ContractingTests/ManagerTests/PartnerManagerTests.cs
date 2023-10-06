using ContractingTests.Extensions;

namespace ContractingTests.ManagerTests;

public class PartnerManagerTests
{
    [Fact]
    public void GetByExternalId_ReturnsPartner_WhenFound()
    {
        // arrange
        (var partnerMgr, var mockPartnerRepo) = GetSutAndMocks();
        string externalId = "PNER-1";

        mockPartnerRepo
            .Setup(x => x.GetWithContractsByExternalId(It.IsAny<string>()))
            .Returns<string>(eid =>
            {
                var partner = ValidNewPartner.Clone();
                partner.Id = 1;
                partner.ExternalId = eid;

                return partner;
            })
            .Verifiable();

        // act
        var partner = partnerMgr
            .GetWithContractsByExternalId(externalId);

        // assert
        Assert.NotNull(partner);
        Assert.Equal(externalId, partner.ExternalId);

        mockPartnerRepo.Verify(
            repo => repo.GetWithContractsByExternalId(externalId),
            Times.Once);
    }

    [Fact]
    public void GetByExternalId_ReturnsNull_WhenNotFound()
    {
        // arrange
        (var partnerMgr, var mockPartnerRepo) = GetSutAndMocks();
        string externalId = "PNER-0";

        mockPartnerRepo
            .Setup(x => x.GetWithContractsByExternalId(externalId))
            .Returns(() => null)
            .Verifiable();

        // act
        var partner = partnerMgr
            .GetWithContractsByExternalId(externalId);

        // assert
        Assert.Null(partner);

        mockPartnerRepo.Verify(
            repo => repo.GetWithContractsByExternalId(externalId),
            Times.Once);
    }

    [Fact]
    public void RegisterPartner_ReturnsSuccess_WhenValid()
    {
        // arrange
        (var partnerMgr, var mockPartnerRepo) = GetSutAndMocks();

        mockPartnerRepo
            .Setup(x => x.GetWithContractsByEmail(ValidNewPartner.Email))
            .Returns(() => null);

        mockPartnerRepo
            .Setup(x => x.Create(It.IsAny<Partner>()))
            .Returns<Partner>(p =>
            {
                var partner = p.Clone();
                partner.Id = 1;
                partner.ExternalId = "PNER-1";

                return partner;
            })
            .Verifiable();

        // act
        var result = partnerMgr.RegisterPartner(ValidNewPartner);

        // assert
        AssertCreated(
            result, ValidNewPartner, mockPartnerRepo);
    }

    [Fact]
    public void RegisterPartner_ReturnsError_WhenPartnerUnder18()
    {
        // arrange
        (var partnerMgr, var mockPartnerRepo) = GetSutAndMocks();
        var partner = ValidNewPartner.WithUnderEighteenAge();
        mockPartnerRepo
            .Setup(x => x.Create(partner))
            .Verifiable();

        // act
        var result = partnerMgr.RegisterPartner(partner);

        // assert
        Assert.IsType<PartnerUnderEighteenError>(result);
        mockPartnerRepo.Verify(repo => repo.Create(partner), Times.Never);
    }

    [Fact]
    public void RegisterPartner_ReturnsError_WhenPartnerEmailAlreadyExists()
    {
        // arrange
        (var partnerMgr, var mockPartnerRepo) = GetSutAndMocks();
        var partner = ExistingPartner.WithUsedEmail();

        mockPartnerRepo
            .Setup(x => x.GetWithContractsByEmail(partner.Email))
            .Returns(() =>
            {
                var existing = ValidNewPartner.Clone();
                existing.Id = 2;
                existing.ExternalId = "PNER-2";

                return existing;
            });

        mockPartnerRepo
            .Setup(x => x.Create(partner))
            .Verifiable();

        // act
        var result = partnerMgr.RegisterPartner(partner);

        // assert
        Assert.IsType<PartnerEmailAlreadyExistsError>(result);
        mockPartnerRepo.Verify(repo => repo.GetWithContractsByEmail(partner.Email), Times.Once);
        mockPartnerRepo.Verify(repo => repo.Create(partner), Times.Never);
    }

    [Fact]
    public void UpdatePartnerInfo_ReturnsSuccess_WhenValid()
    {
        // arrange
        (var partnerMgr, var mockPartnerRepo) = GetSutAndMocks();

        mockPartnerRepo
            .Setup(x => x.GetWithContractsByEmail(It.IsAny<string>()))
            .Returns(() => null);

        mockPartnerRepo
            .Setup(x => x.UpdateInfoByExternalId(It.IsAny<Partner>()))
            .Returns<Partner>(p => p)
            .Verifiable();

        // act
        var result = partnerMgr.UpdatePartnerInfoByExternalId(ExistingPartner);

        // assert
        AssertUpdated(result, ExistingPartner, mockPartnerRepo);
    }

    [Fact]
    public void UpdatePartnerInfo_ReturnsError_WhenPartnerUnder18()
    {
        // arrange
        (var partnerMgr, var mockPartnerRepo) = GetSutAndMocks();
        var partner = ValidNewPartner.WithUnderEighteenAge();
        mockPartnerRepo
            .Setup(x => x.UpdateInfoByExternalId(partner))
            .Verifiable();

        // act
        var result = partnerMgr.UpdatePartnerInfoByExternalId(partner);

        // assert
        Assert.IsType<PartnerUnderEighteenError>(result);
        mockPartnerRepo.Verify(repo => repo.UpdateInfoByExternalId(partner), Times.Never);
    }

    [Fact]
    public void UpdatePartnerInfo_ReturnsError_WhenPartnerEmailAlreadyExists()
    {
        // arrange
        (var partnerMgr, var mockPartnerRepo) = GetSutAndMocks();
        var partner = ExistingPartner.WithUsedEmail();

        mockPartnerRepo
            .Setup(x => x.GetWithContractsByEmail(partner.Email))
            .Returns(() =>
            {
                var existing = ValidNewPartner.Clone();
                existing.Id = 2;
                existing.ExternalId = "PNER-2";

                return existing;
            });

        mockPartnerRepo
            .Setup(x => x.UpdateInfoByExternalId(partner))
            .Verifiable();

        // act
        var result = partnerMgr.UpdatePartnerInfoByExternalId(partner);

        // assert
        Assert.IsType<PartnerEmailAlreadyExistsError>(result);
        mockPartnerRepo.Verify(repo => repo.GetWithContractsByEmail(partner.Email), Times.Once);
        mockPartnerRepo.Verify(repo => repo.UpdateInfoByExternalId(partner), Times.Never);
    }

    [Fact]
    public void UpdatePartnerInfo_ReturnsError_WhenPartnerNotFound()
    {
        // arrange
        (var partnerMgr, var mockPartnerRepo) = GetSutAndMocks();
        var partner = ValidNewPartner.WithInvalidId();

        mockPartnerRepo
            .Setup(x => x.UpdateInfoByExternalId(partner))
            .Returns(() => null)
            .Verifiable();

        // act
        var result = partnerMgr.UpdatePartnerInfoByExternalId(partner);

        // assert
        Assert.IsType<PartnerNotFoundError>(result);
        mockPartnerRepo.Verify(repo => repo.UpdateInfoByExternalId(partner), Times.Once);
    }

    // Constants and helpers
    private static readonly Partner ValidNewPartner = new()
    {
        FirstName = "TestFName",
        LastName = "TestLName",
        Email = "Test@testmail.com",
        PhoneNumber = "0123456789",
        DateOfBirth = new DateTime(1989, 11, 10),
        Gender = Gender.Male
    };

    private static readonly Partner ExistingPartner
        = ValidNewPartner.WithValidId();

    private static void AssertCreated(
        IRegisterPartnerResult result,
        Partner expected,
        Mock<IPartnerRepository> mockPartnerRepo)
    {
        var successResult = Assert.IsType<PartnerSuccessResult>(result);

        Assert.NotNull(successResult.Partner);
        Assert.NotEqual(0, successResult.Partner.Id);
        Assert.Equal(expected.FirstName, successResult.Partner.FirstName);
        Assert.Equal(expected.LastName, successResult.Partner.LastName);
        Assert.Equal(expected.Email, successResult.Partner.Email);
        Assert.Equal(expected.PhoneNumber, successResult.Partner.PhoneNumber);
        Assert.Equal(expected.DateOfBirth, successResult.Partner.DateOfBirth);
        Assert.Equal(expected.Gender, successResult.Partner.Gender);

        mockPartnerRepo.Verify(repo => repo.Create(expected), Times.Once);
    }

    private static void AssertUpdated(
        IUpdatePartnerInfoResult result,
        Partner expected,
        Mock<IPartnerRepository> mockPartnerRepo)
    {
        var successResult = Assert.IsType<PartnerSuccessResult>(result);

        Assert.NotNull(successResult.Partner);
        Assert.Equal(expected.FirstName, successResult.Partner.FirstName);
        Assert.Equal(expected.LastName, successResult.Partner.LastName);
        Assert.Equal(expected.Email, successResult.Partner.Email);
        Assert.Equal(expected.PhoneNumber, successResult.Partner.PhoneNumber);
        Assert.Equal(expected.DateOfBirth, successResult.Partner.DateOfBirth);
        Assert.Equal(expected.Gender, successResult.Partner.Gender);

        mockPartnerRepo.Verify(repo => repo.UpdateInfoByExternalId(expected), Times.Once);
    }

    private static (
        PartnerManager partnerMgr,
        Mock<IPartnerRepository> mockPartnerRepo) GetSutAndMocks()
    {
        var mockPartnerRepo = new Mock<IPartnerRepository>();
        return (new PartnerManager(mockPartnerRepo.Object), mockPartnerRepo);
    }
}
