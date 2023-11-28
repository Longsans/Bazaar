namespace ContractingTests.UnitTests;

public class ContractUnitTests
{
    #region Test data and helpers
    private const int _clientId = 1;
    private const int _sellingPlanId = 1;

    public static IEnumerable<object?[]> ValidEndDates
        => new List<object?[]>
        {
            new object?[] { DateTime.Now.AddDays(30) },
            new object?[] { DateTime.Now },
            new object?[] { null },
        };

    public static IEnumerable<object?[]> InterruptibleEndDates()
    {
        return new List<object?[]>
        {
            new object?[] { DateTime.Now.AddDays(30) },
            new object?[] { null },
        };
    }

    public static IEnumerable<object?[]> UninterruptibleEndDates()
    {
        return new List<object?[]>
        {
            new object?[] { DateTime.Now - TimeSpan.FromDays(30) },
            new object?[] { DateTime.Now },
        };
    }

    public static IEnumerable<object?[]> ExtensibleEndDates
        => new List<object?[]>
        {
            new object?[] { DateTime.Now.AddDays(30) },
            new object?[] { DateTime.Now },
        };
    #endregion

    [Fact]
    public void Constructor_Succeeds_WhenValid()
    {
        var contract = new Contract(_clientId, _sellingPlanId);

        Assert.Equal(DateTime.Now.Date, contract.StartDate);
        Assert.Null(contract.EndDate);
        Assert.Equal(_clientId, contract.ClientId);
        Assert.Equal(_sellingPlanId, contract.SellingPlanId);
    }

    [Fact]
    public void EndContract_Succeeds_WhenValid()
    {
        var contract = new Contract(1, 1);

        contract.End();

        Assert.Equal(DateTime.Now.Date, contract.EndDate);
    }

    [Fact]
    public void EndContract_ThrowsException_WhenContractEnded()
    {
        var contract = new Contract(1, 1);
        contract.End();

        Assert.Throws<ContractEndedException>(contract.End);
    }
}
