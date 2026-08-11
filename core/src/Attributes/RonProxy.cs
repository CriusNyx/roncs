[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
public class RonProxyAttribute(Type proxy) : Attribute
{
  public readonly Type Proxy = proxy;
}
