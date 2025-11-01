using Utilities.Core.Shared.Builders;
using Utilities.Core.Shared.Internals;

namespace Utilities.Core.Shared.Extensions;

public static class GenericMapperExtensions
{
    private static readonly MapperCache _cache = new();

    public static TTarget MapTo<TTarget>(this object source) where TTarget : class
    {
        if(source is null)
            throw new ArgumentNullException(nameof(source));

        var key = (source.GetType(), typeof(TTarget));

        var mapper = _cache.GetOrAdd(key, () =>
            MapperExpressionBuilder.CreateMapper<TTarget>(source.GetType()));

        return ((Func<object, TTarget>)mapper)(source);
    }
}
