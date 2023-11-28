namespace Bazaar.FbbInventory.Domain.Constants;

public static class StoragePolicy
{
    public static TimeSpan MaximumUnfulfillableDuration => TimeSpan.FromDays(30);
    public const string FulfillableLotCodePrefix = "FUFL-";
    public const string UnfulfillableLotCodePrefix = "UNFL-";
}
