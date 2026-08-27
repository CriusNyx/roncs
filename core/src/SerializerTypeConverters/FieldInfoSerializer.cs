using System.Reflection;
using RonCS.AST;

namespace RonCS;

/// <summary>
/// Type converter for a member.s
/// </summary>
/// <param name="field"></param>
/// <param name="converter"></param>
internal class MemberInfoSerializer(MemberInfo field, TypeSerializerConverter? converter)
  : TypeSerializerFieldConverter
{
  private TypeSerializerConverter? converter = converter;

  public RonElement FieldElementForObject(SerializationContext context, object source)
  {
    return new RonNamedValue(
      new RonIdentifier(field.Name),
      converter?.ToAST(context, field.GetMemberValue(source)!)
        ?? context.ToAST(field.GetMemberValue(source), field.MemberValueType())
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
