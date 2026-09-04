using RonCS.AST;

namespace RonCS.Converters;

/// <summary>
/// Converts objects to RON AST trees that can be serialized.
/// </summary>
internal interface TypeSerializerFieldConverter
{
  public RonElement FieldElementForObject(SerializationContext context, object source);
}
