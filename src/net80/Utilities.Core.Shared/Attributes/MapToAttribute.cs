namespace Utilities.Core.Shared.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class MapToAttribute : Attribute
{
    public string TargetProperty { get; }
    public MapToAttribute(string targetProperty) => TargetProperty = targetProperty;
}
