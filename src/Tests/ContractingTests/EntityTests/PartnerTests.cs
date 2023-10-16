

namespace ContractingTests.EntityTests;

public class PartnerTests
{
    private readonly Partner _existingPartner = new(
        "Test", "Test", "test@testmail.com", "0901234567",
        new DateTime(1989, 11, 11), Gender.Male);

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public void CreateConstructor_Succeeds_WhenValid(int yearsAfterMinAge)
    {
        var dob = DateTime.Now.Date.AddYears(-PartnerCompliance.MinimumAge - yearsAfterMinAge);

        var partner = new Partner("Test", "Test", "test@testmail.com",
            "0901234567", dob, Gender.Male);

        Assert.Equal(dob, partner.DateOfBirth);
    }

    [Fact]
    public void CreateConstructor_ThrowsException_WhenPartnerUnderMinimumAge()
    {
        var dob = DateTime.Now.Date.AddYears(-PartnerCompliance.MinimumAge + 1);

        Assert.Throws<PartnerUnderMinimumAgeException>(() =>
        {
            var partner = new Partner("Test", "Test", "test@testmail.com",
                "0901234567", dob, Gender.Male);
        });
    }

    [Fact]
    public void SignContract_Succeeds_WhenValid()
    {
        var contract = new Contract(1, 1, null);

        _existingPartner.SignContract(contract);

        Assert.Equal(contract, _existingPartner.Contracts.Last());
    }

    [Fact]
    public void SignContract_ThrowsException_WhenPartnerUnderContract()
    {
        _existingPartner.SignContract(new Contract(1, 1, null));
        var contract = new Contract(1, 1, null);

        Assert.Throws<PartnerAlreadyUnderContractException>(
            () => _existingPartner.SignContract(contract));
    }

    [Fact]
    public void ChangeEmailAddress_Succeeds_WhenValid()
    {
        string newEmailAddress = "newemail@testmail.com";

        _existingPartner.ChangeEmailAddress(newEmailAddress);

        Assert.Equal(newEmailAddress, _existingPartner.EmailAddress);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void ChangeEmailAddress_ThrowsArgumentNullException_WhenNewEmailAddressNullOrWhiteSpace(
        string newEmailAddress)
    {
        Assert.Throws<ArgumentNullException>(
            () => _existingPartner.ChangeEmailAddress(newEmailAddress));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void CurrentContract_ReturnsContract_WhenPresent(bool indefinite, bool partnerHasMany)
    {
        var latestContract = AddTestContractsWithLatest(
            _existingPartner, indefinite ? null : DateTime.Now.AddMonths(1), partnerHasMany);

        Assert.Equal(latestContract, _existingPartner.CurrentContract);
    }

    [Fact]
    public void CurrentContract_ReturnsNull_WhenNoContract()
    {
        Assert.Null(_existingPartner.CurrentContract);
    }

    [Fact]
    public void CurrentContract_ReturnsNull_WhenAllContractsEnded()
    {
        var _ = AddTestContractsWithLatest(
            _existingPartner, DateTime.Now.AddDays(-1));

        Assert.Null(_existingPartner.CurrentContract);
    }

    [Fact]
    public void IsNotUnderContract_IfCurrentContractEndsToday()
    {
        var latestContract = new Contract(1, 1, DateTime.Now);
        latestContract.SetStartDate(DateTime.Now.AddMonths(-1));
        _existingPartner.SignContract(latestContract);

        Assert.False(_existingPartner.IsUnderContract);
    }

    #region Helpers
    private static Contract AddTestContractsWithLatest(
        Partner partner, DateTime? latestEndDate, bool multiple = false)
    {
        if (multiple)
        {
            var firstContract = new Contract(1, 1, null);
            firstContract.SetStartDate(DateTime.Now.AddMonths(-1));
            firstContract.SetEndDate(DateTime.Now.AddDays(-1));
            partner.SignContract(firstContract);
        }

        var latestContract = new Contract(1, 1,
            latestEndDate > DateTime.Now.Date ? latestEndDate : null);
        latestContract.SetStartDate(DateTime.Now.AddDays(-1));

        if (latestEndDate <= DateTime.Now.Date)
            latestContract.SetEndDate(latestEndDate.Value);

        partner.SignContract(latestContract);

        return latestContract;
    }
    #endregion
}
