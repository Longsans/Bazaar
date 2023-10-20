namespace ShopperInfoTests.UnitTests;

public class ShopperUnitTests
{
    [Theory]
    [InlineData(22)]
    [InlineData(0)]
    public void Constructor_Succeeds_WhenValid(int yearsBeforeNow)
    {
        var dob = DateTime.Now.AddYears(-yearsBeforeNow);
        var shopper = new Shopper("Test", "Test", "test@testmail.com",
            "0123456789", dob, Gender.Male);

        Assert.Equal(dob.Date, shopper.DateOfBirth);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenDateOfBirthInTheFuture()
    {
        var dob = DateTime.Now.AddDays(1);

        Assert.Throws<ArgumentException>(() =>
        {
            var shopper = new Shopper("Test", "Test", "test@testmail.com",
                "0123456789", dob, Gender.Male);
        });
    }
}