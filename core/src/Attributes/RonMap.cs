namespace RonCS;

/// <summary>
/// Marks a class or field as a ron map.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
public class RonMapAttribute : Attribute;
