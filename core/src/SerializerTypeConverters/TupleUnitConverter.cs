namespace RonCS;

internal class TupleUnitConverter(string name) : TypeSerializerConverter
{
  public RonElement ToAST(SerializationContext context, object source)
  {
    return new RonUnitStruct(new RonIdentifier(name));
  }
}
