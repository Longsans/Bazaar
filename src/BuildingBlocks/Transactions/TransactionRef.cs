namespace Bazaar.BuildingBlocks.Transactions;

public record TransactionRef(int requestId, int clusterId) : IFormattable
{
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return $"{requestId}-{clusterId}";
    }
}
