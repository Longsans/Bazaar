namespace ContractingTests.Helpers;

public static class ContractExtensions
{
    public static void SetStartDate(this Contract contract, DateTime startDate)
    {
        typeof(Contract).GetProperty("StartDate")
            .SetValue(contract, startDate.Date);
    }

    public static void SetEndDate(this Contract contract, DateTime endDate)
    {
        typeof(Contract).GetProperty("EndDate")
            .SetValue(contract, endDate.Date);
    }
}
