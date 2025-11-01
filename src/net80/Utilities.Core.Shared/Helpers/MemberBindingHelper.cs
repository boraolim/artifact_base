using Utilities.Core.Shared.Attributes;

namespace Utilities.Core.Shared.Helpers;

public static class MemberBindingHelper
{
    public static List<MemberBinding> CreateBindings<TTarget>(Type sourceType, Expression sourceTyped)
    {
        var bindings = new List<MemberBinding>();
        var targetMembers = typeof(TTarget).GetMembers(BindingFlags.Public | BindingFlags.Instance);

        foreach(var targetMember in targetMembers)
        {
            if(!IsWritableMember(targetMember))
                continue;

            var sourceMember = FindSourceMember(sourceType, targetMember);
            if(sourceMember == null)
                continue;

            var sourceValue = GetValueExpression(sourceTyped, sourceMember);
            var targetType = GetMemberType(targetMember);

            if(sourceValue != null)
            {
                if(sourceValue.Type != targetType)
                    sourceValue = TypeConversionHelper.ConvertExpression(sourceValue, targetType);

                bindings.Add(Expression.Bind(targetMember, sourceValue));
            }
        }

        return bindings;
    }

    public static Expression? GetSourceExpression(Type sourceType, Expression sourceTyped, string name, Type targetType)
    {
        var member = sourceType.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(m => string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase)
                        && ((m is PropertyInfo p && p.CanRead) || m is FieldInfo));

        if(member.CheckIsNull() || member == default)
            throw new InvalidOperationException($"No se encontró un miembro público legible llamado '{name}' en '{sourceType.Name}'.");

        var expr = GetValueExpression(sourceTyped, member);

        // Si el tipo de la expresión no coincide, convertirlo
        return (expr != default && expr.Type != targetType)
                ? TypeConversionHelper.ConvertExpression(expr, targetType)
                : expr;
    }

    private static bool IsWritableMember(MemberInfo member) =>
        (member is PropertyInfo prop && prop.CanWrite) || member is FieldInfo;

    private static MemberInfo? FindSourceMember(Type sourceType, MemberInfo target)
    {
        return sourceType.GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m =>
            {
                if(m.GetCustomAttribute<IgnoreMapAttribute>() != null)
                    return false;

                var mapAttr = m.GetCustomAttribute<MapToAttribute>();
                if(mapAttr != null)
                    return string.Equals(mapAttr.TargetProperty, target.Name, StringComparison.OrdinalIgnoreCase);

                return string.Equals(m.Name, target.Name, StringComparison.OrdinalIgnoreCase);
            });
    }

    private static Expression? GetValueExpression(Expression source, MemberInfo member)
    {
        source = Guard.AgainstNull<Expression>(source, nameof(source));
        member = Guard.AgainstNull<MemberInfo>(member, nameof(member));

        return member switch
        {
            PropertyInfo p when p.CanRead => Expression.Property(source, p),
            FieldInfo f => Expression.Field(source, f),
            _ => default // Si no es propiedad ni campo
        };
    }

    private static Type GetMemberType(MemberInfo member) =>
        member switch
        {
            PropertyInfo p => p.PropertyType,
            FieldInfo f => f.FieldType,
            _ => throw new InvalidOperationException()
        };

}

