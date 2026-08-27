using RonCS.AST;

namespace RonCS;

/// <summary>
/// Converts objects to RON AST trees that can be serialized.
/// </summary>
internal interface TypeSerializerFieldConverter
{
  public RonElement FieldElementForObject(SerializationContext context, object source);
}
