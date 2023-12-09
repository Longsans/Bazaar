namespace Bazaar.FbbInventory.Domain.Constants;

public static class StoragePolicy
{
    public static TimeSpan MaximumUnfulfillableDuration => TimeSpan.FromDays(30);
    public const string LotCodePrefix = "LOT-";
}
