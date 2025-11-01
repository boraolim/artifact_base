using Utilities.Core.Shared.Helpers;

namespace Utilities.Core.Shared.Builders;

internal static class MapperExpressionBuilder
{
    public static Func<object, TTarget> CreateMapper<TTarget>(Type sourceType) where TTarget : class
    {
        var sourceParam = Expression.Parameter(typeof(object), "source");
        var sourceTyped = Expression.Convert(sourceParam, sourceType);

        var ctorExpr = CreateConstructorExpression<TTarget>(sourceType, sourceTyped);
        var bindings = MemberBindingHelper.CreateBindings<TTarget>(sourceType, sourceTyped);

        Expression body = bindings.Count == 0
            ? ctorExpr
            : Expression.MemberInit(ctorExpr, bindings);

        return Expression.Lambda<Func<object, TTarget>>(body, sourceParam).Compile();
    }

    private static NewExpression CreateConstructorExpression<TTarget>(Type sourceType, Expression sourceTyped)
    {
        var constructor = typeof(TTarget).GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault()
            ?? throw new InvalidOperationException($"La clase {typeof(TTarget).Name} no tiene un constructor público.");

        var args = constructor.GetParameters()
            .Select(p =>
                MemberBindingHelper.GetSourceExpression(sourceType, sourceTyped, p.Name, p.ParameterType)
                ?? Expression.Default(p.ParameterType))
            .ToArray();

        return Expression.New(constructor, args);
    }
}

