using Hogar.Core.Shared.Attributes;

namespace Hogar.Core.Shared.Extensions;

public static class GenericMapperExtensions
{
    private static readonly object _lock = new();
    private static readonly Dictionary<(Type, Type), Delegate> _cache = new();

    public static TTarget MapTo<TTarget>(this object source) where TTarget : class
    {
        if(source.CheckIsNull())
            throw new ArgumentNullException(nameof(source));

        var key = (source.GetType(), typeof(TTarget));

        if(!_cache.TryGetValue(key, out var cachedMapper))
        {
            lock(_lock)
            {
                if(!_cache.TryGetValue(key, out cachedMapper))
                {
                    cachedMapper = CreateMapper<TTarget>(source.GetType());
                    _cache[key] = cachedMapper;
                }
            }
        }
        return ((Func<object, TTarget>)cachedMapper)(source);
    }

    #region "Métodos privados"

    private static Func<object, TTarget> CreateMapper<TTarget>(Type sourceType) where TTarget : class
    {
        var sourceParam = Expression.Parameter(typeof(object), "source");
        var sourceTyped = Expression.Convert(sourceParam, sourceType);
        var newInstance = CreateConstructorExpression<TTarget>(sourceType, sourceTyped);
        var bindings = CreateMemberBindings<TTarget>(sourceType, sourceTyped);

        Expression body = bindings.Count == 0
            ? (Expression)newInstance
            : Expression.MemberInit(newInstance, bindings);

        return Expression.Lambda<Func<object, TTarget>>(body, sourceParam).Compile();
    }

    private static NewExpression CreateConstructorExpression<TTarget>(Type sourceType, Expression sourceTyped)
    {
        var constructor = typeof(TTarget).GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault()
            ?? throw new InvalidOperationException($"La clase {typeof(TTarget).Name} no tiene un constructor público.");

        var args = constructor.GetParameters()
            .Select(param =>
            {
                var expr = GetSourceMemberExpression(sourceType, sourceTyped, param.Name, param.ParameterType);
                return expr ?? Expression.Default(param.ParameterType);
            })
            .ToArray();

        return Expression.New(constructor, args);
    }

    private static List<MemberBinding> CreateMemberBindings<TTarget>(Type sourceType, Expression sourceTyped)
    {
        var bindings = new List<MemberBinding>();

        foreach(var targetMember in typeof(TTarget).GetMembers(BindingFlags.Public | BindingFlags.Instance))
        {
            if((targetMember is PropertyInfo prop && prop.CanWrite) || targetMember is FieldInfo)
            {
                var targetType = GetMemberType(targetMember);

                // Buscar en el origen un miembro con [MapTo] que apunte a este miembro destino
                var sourceMember = sourceType.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(m =>
                    {
                        if(m.GetCustomAttribute<IgnoreMapAttribute>() != null)
                            return false;

                        var mapAttr = m.GetCustomAttribute<MapToAttribute>();
                        if(mapAttr != null)
                            return string.Equals(mapAttr.TargetProperty, targetMember.Name, StringComparison.OrdinalIgnoreCase);

                        return string.Equals(m.Name, targetMember.Name, StringComparison.OrdinalIgnoreCase);
                    });

                if(sourceMember == null)
                    continue;

                Expression sourceValue = sourceMember switch
                {
                    PropertyInfo p => Expression.Property(sourceTyped, p),
                    FieldInfo f => Expression.Field(sourceTyped, f),
                    _ => null
                };

                if(sourceValue != null)
                {
                    if(sourceValue.Type != targetType)
                        sourceValue = ConvertExpression(sourceValue, targetType);

                    bindings.Add(Expression.Bind(targetMember, sourceValue));
                }
            }
        }

        return bindings;
    }


    private static Expression GetSourceMemberExpression(Type sourceType, Expression sourceTyped, string memberName, Type targetType)
    {
        var sourceMember = sourceType.GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(methodObj => string.Equals(methodObj.Name, memberName, StringComparison.OrdinalIgnoreCase) &&
                           ((methodObj is PropertyInfo pInfo && pInfo.CanRead) || methodObj is FieldInfo));

        if(sourceMember == null) return null;

        Expression sourceValue = sourceMember switch
        {
            PropertyInfo prop => Expression.Property(sourceTyped, prop),
            FieldInfo field => Expression.Field(sourceTyped, field),
            _ => null
        };

        if(sourceValue != null && sourceValue.Type != targetType)
            return ConvertExpression(sourceValue, targetType);

        return sourceValue;
    }

    private static Expression ConvertExpression(Expression source, Type targetType)
    {
        if(targetType.IsAssignableFrom(source.Type))
            return source;

        if(typeof(IEnumerable).IsAssignableFrom(targetType) && targetType.IsGenericType)
        {
            Type sourceItemType = source.Type.GetGenericArguments()[0];
            Type targetItemType = targetType.GetGenericArguments()[0];

            if(typeof(IDictionary).IsAssignableFrom(targetType))
                return ConvertDictionary(source, targetType);

            return ConvertCollection(source, sourceItemType, targetItemType);
        }

        return Expression.Convert(source, targetType);
    }
    private static Expression ConvertCollection(Expression source, Type sourceItemType, Type targetItemType)
    {
        if(targetItemType.IsAssignableFrom(sourceItemType)) return source;

        var selectMethod = GetGenericMethod(typeof(Enumerable), "Select", 2, sourceItemType, targetItemType);
        var toListMethod = GetGenericMethod(typeof(Enumerable), "ToList", 1, targetItemType);
        var param = Expression.Parameter(sourceItemType, "x");
        var itemConversion = ConvertExpression(param, targetItemType);
        var selectExpr = Expression.Call(selectMethod, source, Expression.Lambda(itemConversion, param));

        return Expression.Call(toListMethod, selectExpr);
    }

    private static Expression ConvertDictionary(Expression source, Type targetType)
    {
        var sourceArgs = source.Type.GetGenericArguments();
        var sourceKeyType = sourceArgs[0];
        var sourceValueType = sourceArgs[1];

        var targetArgs = targetType.GetGenericArguments();
        var targetKeyType = targetArgs[0];
        var targetValueType = targetArgs[1];

        var sourceKvpType = typeof(KeyValuePair<,>).MakeGenericType(sourceKeyType, sourceValueType);

        var param = Expression.Parameter(sourceKvpType, "kv");

        // conversiones explícitas key/value
        var keyExpr = ConvertExpression(Expression.Property(param, "Key"), targetKeyType);
        var valExpr = ConvertExpression(Expression.Property(param, "Value"), targetValueType);

        var keySelector = Expression.Lambda(keyExpr, param);     // Func<KVP, newKey>
        var valueSelector = Expression.Lambda(valExpr, param);   // Func<KVP, newValue>

        // método ToDictionary<TSource, TKey, TElement>
        var toDictionaryMethod = typeof(Enumerable).GetMethods()
            .First(methodObj => methodObj.Name == "ToDictionary" &&
                        methodObj.IsGenericMethodDefinition &&
                        methodObj.GetGenericArguments().Length == 3 &&
                        methodObj.GetParameters().Length == 3)
            .MakeGenericMethod(sourceKvpType, targetKeyType, targetValueType);

        return Expression.Call(toDictionaryMethod, source, keySelector, valueSelector);
    }

    private static MethodInfo GetGenericMethod(Type type, string methodName, int genericArgCount, params Type[] typeArgs)
    {
        return type.GetMethods()
            .Where(methodObj => methodObj.Name == methodName && methodObj.IsGenericMethodDefinition)
            .First(methodObj =>
            {
                var genArgs = methodObj.GetGenericArguments().Length == genericArgCount;
                var paramsCount = methodObj.GetParameters().Length == typeArgs.Length;
                return genArgs && paramsCount;
            })
            .MakeGenericMethod(typeArgs);
    }


    private static Type GetMemberType(MemberInfo member) =>
        member switch
        {
            PropertyInfo prop => prop.PropertyType,
            FieldInfo field => field.FieldType,
            _ => throw new InvalidOperationException()
        };

    #endregion
}
