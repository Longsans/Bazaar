namespace ContractingTests.UnitTests;

public class ClientUnitTests
{
    private readonly Client _existingClient = new(
        "Test", "Test", "test@testmail.com", "0901234567",
        new DateTime(1989, 11, 11), Gender.Male);

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public void CreateConstructor_Succeeds_WhenValid(int yearsAfterMinAge)
    {
        var dob = DateTime.Now.Date.AddYears(-ClientCompliance.MinimumAge - yearsAfterMinAge);

        var client = new Client("Test", "Test", "test@testmail.com",
            "0901234567", dob, Gender.Male);

        Assert.Equal(dob, client.DateOfBirth);
    }

    [Fact]
    public void CreateConstructor_ThrowsException_WhenClientUnderMinimumAge()
    {
        var dob = DateTime.Now.Date.AddYears(-ClientCompliance.MinimumAge + 1);

        Assert.Throws<ClientUnderMinimumAgeException>(() =>
        {
            var client = new Client("Test", "Test", "test@testmail.com",
                "0901234567", dob, Gender.Male);
        });
    }

    [Fact]
    public void SignContract_Succeeds_WhenValid()
    {
        var contract = new Contract(1, 1);

        _existingClient.SignContract(contract);

        Assert.Equal(contract, _existingClient.Contracts.Last());
    }

    [Fact]
    public void SignContract_ThrowsException_WhenClientUnderContract()
    {
        _existingClient.SignContract(new Contract(1, 1));
        var contract = new Contract(1, 1);

        Assert.Throws<ClientAlreadyUnderContractException>(
            () => _existingClient.SignContract(contract));
    }

    [Fact]
    public void ChangeEmailAddress_Succeeds_WhenValid()
    {
        string newEmailAddress = "newemail@testmail.com";

        _existingClient.ChangeEmailAddress(newEmailAddress);

        Assert.Equal(newEmailAddress, _existingClient.EmailAddress);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void ChangeEmailAddress_ThrowsArgumentNullException_WhenNewEmailAddressNullOrWhiteSpace(
        string newEmailAddress)
    {
        Assert.Throws<ArgumentNullException>(
            () => _existingClient.ChangeEmailAddress(newEmailAddress));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CurrentContract_ReturnsContract_WhenPresent(bool clientHasMany)
    {
        var latestContract = AddTestContractsWithLatest(
            _existingClient, false, clientHasMany);

        Assert.Equal(latestContract, _existingClient.CurrentContract);
    }

    [Fact]
    public void CurrentContract_ReturnsNull_WhenNoContract()
    {
        Assert.Null(_existingClient.CurrentContract);
    }

    [Fact]
    public void CurrentContract_ReturnsNull_WhenAllContractsEnded()
    {
        var _ = AddTestContractsWithLatest(
            _existingClient, true);

        Assert.Null(_existingClient.CurrentContract);
    }

    [Fact]
    public void IsNotUnderContract_IfCurrentContractEndsToday()
    {
        var latestContract = new Contract(1, 1);
        latestContract.SetStartDate(DateTime.Now.AddMonths(-1));
        latestContract.End();
        _existingClient.SignContract(latestContract);

        Assert.False(_existingClient.IsUnderContract);
    }

    #region Helpers
    private static Contract AddTestContractsWithLatest(
        Client client, bool allEnded = false, bool multiple = false)
    {
        if (multiple)
        {
            var firstContract = new Contract(1, 1);
            firstContract.SetStartDate(DateTime.Now.AddMonths(-1));
            firstContract.SetEndDate(DateTime.Now.AddDays(-1));
            client.SignContract(firstContract);
        }

        var latestContract = new Contract(1, 1);
        latestContract.SetStartDate(DateTime.Now.AddDays(-1));
        if (allEnded)
        {
            latestContract.End();
        }

        client.SignContract(latestContract);

        return latestContract;
    }
    #endregion
}
