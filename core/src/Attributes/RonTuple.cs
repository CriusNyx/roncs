namespace RonCS;

/// <summary>
/// Marks a method that should be used to serialize the element as a ron tuple.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class RonTupleAttribute : Attribute;
