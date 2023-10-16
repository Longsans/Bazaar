namespace ContractingTests.EntityTests;

public class ContractTests
{
    #region Test data and helpers
    private const int _partnerId = 1;
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

    [Theory]
    [MemberData(nameof(ValidEndDates))]
    public void CreateConstructor_Succeeds_WhenValid(DateTime? endDate)
    {
        var contract = new Contract(_partnerId, _sellingPlanId, endDate);

        Assert.Equal(DateTime.Now.Date, contract.StartDate);
        Assert.Equal(endDate?.Date, contract.EndDate);
        Assert.Equal(_partnerId, contract.PartnerId);
        Assert.Equal(_sellingPlanId, contract.SellingPlanId);
    }

    [Fact]
    public void CreateConstructor_ThrowsException_WhenEndDateBeforeCurrentDate()
    {
        Assert.Throws<EndDateBeforeCurrentDateException>(() =>
        {
            var contract = new Contract(_partnerId, _sellingPlanId,
                DateTime.Now - TimeSpan.FromDays(1));
        });
    }

    [Theory]
    [MemberData(nameof(InterruptibleEndDates))]
    public void EndContract_Succeeds_WhenValid(DateTime? currentEndDate)
    {
        var contract = new Contract(1, 1, currentEndDate);

        contract.End();

        Assert.Equal(DateTime.Now.Date, contract.EndDate);
    }

    [Theory]
    [MemberData(nameof(UninterruptibleEndDates))]
    public void EndContract_ThrowsException_WhenContractEnded(DateTime currentEndDate)
    {
        var contract = new Contract(1, 1, null);
        contract.SetStartDate(currentEndDate - TimeSpan.FromDays(30));
        contract.SetEndDate(currentEndDate);

        Assert.Throws<ContractEndedException>(contract.End);
    }

    [Theory]
    [MemberData(nameof(ExtensibleEndDates))]
    public void ExtendContract_Succeeds_WhenValid(DateTime currentEndDate)
    {
        var contract = new Contract(1, 1, currentEndDate);
        var extendedEndDate = currentEndDate.AddDays(30);

        contract.Extend(extendedEndDate);

        Assert.Equal(extendedEndDate.Date, contract.EndDate);
    }

    [Fact]
    public void ExtendContract_ThrowsException_WhenExtendIndefiniteContract()
    {
        var contract = new Contract(1, 1, null);
        var extendedEndDate = DateTime.Now.AddDays(30);

        Assert.Throws<ExtendIndefiniteContractException>(
            () => contract.Extend(extendedEndDate));
    }

    [Fact]
    public void ExtendContract_ThrowsException_WhenContractEnded()
    {
        var contract = new Contract(1, 1, null);
        contract.SetStartDate(DateTime.Now - TimeSpan.FromDays(30));
        contract.SetEndDate(DateTime.Now - TimeSpan.FromDays(1));

        var extendedEndDate = DateTime.Now.AddDays(30);

        Assert.Throws<ContractEndedException>(() => contract.Extend(extendedEndDate));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public void ExtendContract_ThrowsException_WhenExtendedEndDateNotAfterOriginalEndDate(
        int daysBeforeOriginal)
    {
        var contract = new Contract(1, 1, DateTime.Now.AddDays(30));

        var extendedEndDate = contract.EndDate.Value - TimeSpan.FromDays(daysBeforeOriginal);

        Assert.Throws<ExtendedEndDateNotAfterOriginalEndDateException>(
            () => contract.Extend(extendedEndDate));
    }
}
