using System.Linq.Expressions;

namespace UniThesis.Domain.Specifications
{
    public enum OrderType
    {
        OrderBy,
        OrderByDescending,
        ThenBy,
        ThenByDescending
    }

    public sealed record OrderExpression<T>(
        Expression<Func<T, object>> Expression,
        OrderType OrderType
    );
}
