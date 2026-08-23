using RonCS.AST;

namespace RonCS;

internal interface TypeSerializerFieldConverter
{
  public RonElement FieldElementForObject(SerializationContext context, object source);
}
