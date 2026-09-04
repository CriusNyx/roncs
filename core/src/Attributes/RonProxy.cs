namespace RonCS;

/// <summary>
/// Indicates that a class should be converted to or from the proxy type for serialization.
/// </summary>
/// <param name="proxy"></param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
public class RonProxyAttribute(Type proxy) : Attribute
{
  /// <summary>
  /// The type that provides the proxy for this type.
  /// </summary>
  public readonly Type Proxy = proxy;
}
