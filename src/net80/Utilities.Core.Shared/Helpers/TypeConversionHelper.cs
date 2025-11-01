namespace Utilities.Core.Shared.Helpers;

public static class TypeConversionHelper
{
    public static Expression ConvertExpression(Expression source, Type targetType)
    {
        if(targetType.IsAssignableFrom(source.Type))
            return source;

        // Manejo de nullables y referencias null
        if(!source.Type.IsValueType)
        {
            var temp = Expression.Variable(source.Type, "temp");
            var assign = Expression.Assign(temp, source);
            var condition = Expression.Condition(
                Expression.Equal(temp, Expression.Constant(null, source.Type)),
                Expression.Constant(null, targetType),
                ConvertExpressionNonNull(temp, targetType)
            );
            return Expression.Block(new[] { temp }, assign, condition);
        }

        return ConvertExpressionNonNull(source, targetType);
    }

    private static Expression ConvertExpressionNonNull(Expression source, Type targetType)
    {
        source = Guard.AgainstNull<Expression>(source, nameof(source));
        targetType = Guard.AgainstNull<Type>(targetType, nameof(targetType));

        // 1️⃣ Si el tipo destino es asignable desde el tipo fuente
        if(targetType.IsAssignableFrom(source.Type))
            return source;

        // 2️⃣ Si es colección genérica
        if(typeof(IEnumerable).IsAssignableFrom(targetType) && targetType.IsGenericType)
        {
            Type sourceItemType = source.Type.GetGenericArguments()[0];
            Type targetItemType = targetType.GetGenericArguments()[0];

            if(typeof(IDictionary).IsAssignableFrom(targetType))
                return ConvertDictionary(source, targetType);

            return ConvertCollection(source, sourceItemType, targetItemType);
        }

        // 3️⃣ Si es tipo complejo/clase
        if(source.Type.IsClass && !source.Type.IsPrimitive && source.Type != typeof(string))
        {
            var mapToMethod = typeof(GenericMapperExtensions)
                .GetMethod("MapTo", BindingFlags.Public | BindingFlags.Static)?
                .MakeGenericMethod(targetType);

            if(mapToMethod != null)
                return Expression.Call(mapToMethod, source);
        }

        // 4️⃣ Tipos simples
        return Expression.Convert(source, targetType);
    }

    private static Expression ConvertCollection(Expression source, Type sourceItemType, Type targetItemType)
    {
        // Caso 1: el tipo de destino es asignable directamente al tipo de fuente
        if(targetItemType.IsAssignableFrom(sourceItemType))
            return source;

        // Caso 2: Si la fuente es IEnumerable<T> y destino también, y T es compatible, no hacer nada
        if(typeof(IEnumerable<>).MakeGenericType(sourceItemType).IsAssignableFrom(targetItemType))
            return source;

        var selectMethod = typeof(Enumerable).GetMethods()
            .Where(m => m.Name == "Select" && m.IsGenericMethodDefinition)
            .First(m => m.GetParameters().Length == 2)
            .MakeGenericMethod(sourceItemType, targetItemType);

        var toListMethod = typeof(Enumerable).GetMethods()
            .Where(m => m.Name == "ToList" && m.IsGenericMethodDefinition)
            .First(m => m.GetParameters().Length == 1)
            .MakeGenericMethod(targetItemType);

        var param = Expression.Parameter(sourceItemType, "x");
        var itemConversion = ConvertExpression(param, targetItemType);
        var selectExpr = Expression.Call(selectMethod, source, Expression.Lambda(itemConversion, param));

        return Expression.Call(toListMethod, selectExpr);
    }

    private static Expression ConvertDictionary(Expression source, Type targetType)
    {
        var sourceArgs = source.Type.GetGenericArguments();
        var targetArgs = targetType.GetGenericArguments();

        var kvpType = typeof(KeyValuePair<,>).MakeGenericType(sourceArgs);
        var param = Expression.Parameter(kvpType, "kv");

        var keyExpr = ConvertExpression(Expression.Property(param, "Key"), targetArgs[0]);
        var valExpr = ConvertExpression(Expression.Property(param, "Value"), targetArgs[1]);

        var keySelector = Expression.Lambda(keyExpr, param);
        var valSelector = Expression.Lambda(valExpr, param);

        // ✅ AsEnumerable<KeyValuePair<TKey, TValue>>(source)
        var asEnumerable = Expression.Call(
            typeof(Enumerable),
            "AsEnumerable",
            new Type[] { kvpType }, // 👈 tipo correcto
            source
        );

        // Buscar la sobrecarga ToDictionary<TSource, TKey, TElement>(IEnumerable<TSource>, Func<TSource,TKey>, Func<TSource,TElement>)
        var toDictMethod = typeof(Enumerable).GetMethods()
            .Where(m => m.Name == "ToDictionary" && m.IsGenericMethodDefinition)
            .First(m =>
            {
                var genArgsCount = m.GetGenericArguments().Length;
                var parameters = m.GetParameters();

                if(genArgsCount != 3) return false;          // queremos 3 genéricos
                if(parameters.Length != 3) return false;     // queremos la sobrecarga con 3 parámetros

                // el 2º y 3º parámetro deben ser Func<,> (keySelector, elementSelector)
                var p1 = parameters[1].ParameterType;
                var p2 = parameters[2].ParameterType;

                return p1.IsGenericType && p1.GetGenericTypeDefinition() == typeof(Func<,>)
                    && p2.IsGenericType && p2.GetGenericTypeDefinition() == typeof(Func<,>);
            })
            .MakeGenericMethod(kvpType, targetArgs[0], targetArgs[1]);

        // Llamamos a ToDictionary(asEnumerable, keySelector, valSelector)
        return Expression.Call(toDictMethod, asEnumerable, keySelector, valSelector);
    }

    private static MethodInfo GetGenericMethod(Type type, string name, int genericArgs, params Type[]? typeArgs) =>
        (type.GetMethods()
            .Where(m => m.Name == name && m.IsGenericMethodDefinition)
            .First(m => m.GetGenericArguments().Length == genericArgs))
        is MethodInfo genericDefinition
            ? (typeArgs == null || typeArgs.Length == 0
                ? genericDefinition
                : genericDefinition.MakeGenericMethod(typeArgs))
            : throw new InvalidOperationException($"No se encontró el método genérico {name} con {genericArgs} parámetros genéricos.");

}

