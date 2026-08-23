using RonCS.AST;

namespace RonCS;

internal class ObjectSerializerConverter(
  string objectName,
  TypeSerializerFieldConverter[] converters
) : TypeSerializerConverter
{
  public RonElement ToAST(SerializationContext context, object source)
  {
    var fields = converters.Select(x => x.FieldElementForObject(context, source)).ToArray();
    return new RonNamedValueStruct(new RonIdentifier(objectName), fields);
  }
}
