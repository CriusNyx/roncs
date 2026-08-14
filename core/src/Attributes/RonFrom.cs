/// <summary>
/// Marks a method as a candidate for RonFrom conversion when Serializing or Deserializing.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RonFromAttribute : Attribute;
