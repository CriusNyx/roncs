namespace RonCS;

internal class ProxyConverter(Type target, TypeSerializerConverter targetConverter)
  : TypeSerializerConverter
{
  public readonly Type target = target;
  public readonly TypeSerializerConverter targetConverter = targetConverter;

  public RonElement ToAST(SerializationContext context, object source)
  {
    return targetConverter.ToAST(context, source.RonConvert(target));
  }
}
