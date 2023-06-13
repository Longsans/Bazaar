using System.Diagnostics.CodeAnalysis;

namespace Bazaar.BuildingBlocks.Transactions;

public record TransactionRef(int RequestId, int ClusterId) : IFormattable, IParsable<TransactionRef>
{
    public static TransactionRef Parse(string s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out var transactionRef))
            throw new ArgumentException("Cannot parse string to TransactionRef");
        return transactionRef;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out TransactionRef result)
    {
        if (s is null)
        {
            result = new TransactionRef(default, default);
            return false;
        }
        var elements = s.TrimEnd().Split('-');
        if (elements.Length != 2 || !int.TryParse(elements[0], out int requestId) || !int.TryParse(elements[1], out int clusterId))
        {
            result = new TransactionRef(default, default);
            return false;
        }
        result = new TransactionRef(requestId, clusterId);
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return $"{RequestId}-{ClusterId}";
    }
}
