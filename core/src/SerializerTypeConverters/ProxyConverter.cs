using RonCS.AST;

namespace RonCS.Converters;

/// <summary>
/// Type converter for proxy.
/// </summary>
/// <param name="proxyType"></param>
/// <param name="targetConverter"></param>
internal class ProxyConverter(Type proxyType, TypeSerializerConverter targetConverter)
  : TypeSerializerConverter
{
  public readonly Type target = proxyType;
  public readonly TypeSerializerConverter targetConverter = targetConverter;

  public RonElement ToAST(SerializationContext context, object source)
  {
    return targetConverter.ToAST(context, source.RonConvert(target));
  }
}
