namespace RonCS;

/// <summary>
/// Element should always be serialized, regardless of visibility.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class RonIncludeAttribute : Attribute;
