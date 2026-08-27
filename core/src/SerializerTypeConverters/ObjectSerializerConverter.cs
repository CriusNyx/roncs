using RonCS.AST;

namespace RonCS;

/// <summary>
/// Type converter for class types.
/// </summary>
/// <param name="objectName"></param>
/// <param name="converters"></param>
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
