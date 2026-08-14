using System.Reflection;

namespace RonCS;

internal class FieldInfoSerializer(FieldInfo field, TypeSerializerConverter? converter)
  : TypeSerializerFieldConverter
{
  private TypeSerializerConverter? converter = converter;

  public RonElement FieldElementForObject(SerializationContext context, object source)
  {
    return new RonNamedValue(
      new RonIdentifier(field.Name),
      converter?.ToAST(context, field.GetValue(source)!) ?? context.ToAST(field.GetValue(source))
    );
  }

  public static TypeSerializerConverter? GetFieldConverterForFieldType(Type type)
  {
    if (type.IsListType())
    {
      return new ListConverter();
    }
    return null;
  }
}
