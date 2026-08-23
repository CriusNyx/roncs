namespace RonCS;

/// <summary>
/// Marks a class or field as a list.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
public class RonListAttribute : Attribute;
