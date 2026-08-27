namespace RonCS;

/// <summary>
/// Field should not be serialized in a ron document.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class RonExcludeAttribute : Attribute;
