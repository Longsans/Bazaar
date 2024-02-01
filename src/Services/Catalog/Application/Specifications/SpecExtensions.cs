namespace Bazaar.Catalog.Application.Specifications;

public static class SpecExtensions
{
    public static Specification<T> Combine<T>(this Specification<T> current, Specification<T> spec)
    {
        ((List<IncludeExpressionInfo>)current.IncludeExpressions).AddRange(spec.IncludeExpressions);
        ((List<WhereExpressionInfo<T>>)current.WhereExpressions).AddRange(spec.WhereExpressions);
        ((List<OrderExpressionInfo<T>>)current.WhereExpressions).AddRange(spec.OrderExpressions);
        return current;
    }
}
