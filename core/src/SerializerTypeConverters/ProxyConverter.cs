namespace RonCS;

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
