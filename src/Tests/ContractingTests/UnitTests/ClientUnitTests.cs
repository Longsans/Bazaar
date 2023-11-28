namespace ContractingTests.UnitTests;

public class ClientUnitTests
{
    private readonly Client _existingClient;
    private readonly SellingPlan _testPlan;
    private readonly SellingPlan _newPlan;

    public ClientUnitTests()
    {
        _testPlan = new(1, "Individual", 0m, 1.99m, 0.05f);
        _existingClient = new(
            "Test", "Test", "test@testmail.com", "0901234567",
            new DateTime(1989, 11, 11), Gender.Male, _testPlan.Id);
        _newPlan = new(2, "Business", 40m, 0m, 0.01f);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public void CreateConstructor_Succeeds_WhenValid(int yearsAfterMinAge)
    {
        var dob = DateTime.Now.Date.AddYears(-ClientCompliance.MinimumAge - yearsAfterMinAge);

        var client = new Client("Test", "Test", "test@testmail.com",
            "0901234567", dob, Gender.Male, _testPlan.Id);

        Assert.Equal(dob, client.DateOfBirth);
        Assert.Equal(_testPlan.Id, client.SellingPlanId);
        Assert.NotEmpty(client.Contracts);
    }

    [Fact]
    public void CreateConstructor_ThrowsException_WhenClientUnderMinimumAge()
    {
        var dob = DateTime.Now.Date.AddYears(-ClientCompliance.MinimumAge + 1);

        Assert.Throws<ClientUnderMinimumAgeException>(() =>
        {
            var client = new Client("Test", "Test", "test@testmail.com",
                "0901234567", dob, Gender.Male, _testPlan.Id);
        });
    }

    [Fact]
    public void ChangeSellingPlan_Succeeds_WhenValid()
    {
        var originalContract = _existingClient.CurrentContract;
        _existingClient.ChangeSellingPlan(_newPlan);

        Assert.Equal(_newPlan, _existingClient.SellingPlan);
        Assert.NotEqual(originalContract, _existingClient.CurrentContract);
        Assert.Equal(DateTime.Now.Date, originalContract.EndDate);
    }

    [Fact]
    public void ChangePersonalInfo_Succeeds_WhenValid()
    {
        var newFirstName = "FirstNameTest";
        var newLastName = "LastNameTest";
        var newPhoneNumber = "0123456789";
        var newDob = new DateTime(1999, 11, 11);
        var newGender = Gender.Female;

        _existingClient.ChangePersonalInfo(newFirstName, newLastName,
            newPhoneNumber, newDob, newGender);

        Assert.Equal(newFirstName, _existingClient.FirstName);
        Assert.Equal(newLastName, _existingClient.LastName);
        Assert.Equal(newPhoneNumber, _existingClient.PhoneNumber);
        Assert.Equal(newDob, _existingClient.DateOfBirth);
        Assert.Equal(newGender, _existingClient.Gender);
    }

    [Fact]
    public void ChangePersonalInfo_ThrowsException_WhenClientUnderMinimumAge()
    {
        var newFirstName = "FirstNameTest";
        var newLastName = "LastNameTest";
        var newPhoneNumber = "0123456789";
        var newDob = DateTime.Now.Date.AddYears(-ClientCompliance.MinimumAge + 1);
        var newGender = Gender.Female;

        Assert.Throws<ClientUnderMinimumAgeException>(() =>
        {
            _existingClient.ChangePersonalInfo(newFirstName, newLastName,
                newPhoneNumber, newDob, newGender);
        });
        Assert.NotEqual(newFirstName, _existingClient.FirstName);
        Assert.NotEqual(newLastName, _existingClient.LastName);
        Assert.NotEqual(newPhoneNumber, _existingClient.PhoneNumber);
        Assert.NotEqual(newDob, _existingClient.DateOfBirth);
        Assert.NotEqual(newGender, _existingClient.Gender);
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
        Assert.NotEqual(newEmailAddress, _existingClient.EmailAddress);
    }

    [Fact]
    public void CurrentContract_AlwaysReturnsPresentContract()
    {
        Assert.Equal(_testPlan.Id, _existingClient.CurrentContract.SellingPlanId);
        Assert.Equal(_existingClient.Id, _existingClient.CurrentContract.ClientId);
    }
}
