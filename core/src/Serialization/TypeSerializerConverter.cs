using System.Collections;
using System.Reflection;
using RonCS.AST;

namespace RonCS;

public interface TypeSerializerConverter
{
  public RonElement ToAST(SerializationContext context, object source);

  public static TypeSerializerConverter? CreateTypeConverterFromTypeAttrs(Type type)
  {
    if (type.GetCustomAttribute<RonListAttribute>() is RonListAttribute)
    {
      return new ListConverter();
    }
    if (type.GetCustomAttribute<RonProxyAttribute>() is RonProxyAttribute proxy)
    {
      return new ProxyConverter(
        proxy.Proxy,
        CreateTypeConverterForTypeWithFields(proxy.Proxy, type.Name)
      );
    }
    if (
      type.GetMethods().FirstOrDefault(x => x.GetCustomAttribute<RonTupleAttribute>() != null)
      is MethodInfo method
    )
    {
      return new TupleMethodConverter(type.Name, method);
    }
    if (
      type.GetProperties().FirstOrDefault(x => x.GetCustomAttribute<RonTupleAttribute>() != null)
      is PropertyInfo property
    )
    {
      return new TuplePropertyConverter(type.Name, property);
    }

    return null;
  }

  public static TypeSerializerConverter? CreateTypeConverterForList(Type type)
  {
    // Check if it's a native array.
    if (type.IsArray)
    {
      return new ListConverter();
    }
    // Check if it's a kind of array.
    if (type.IsGenericType)
    {
      var genericType = type.GetGenericTypeDefinition();
      if (genericType == typeof(List<>))
      {
        return new ListConverter();
      }
    }
    if (type == typeof(ArrayList))
    {
      return new ListConverter();
    }
    return null;
  }

  public static TypeSerializerConverter? CreateTypeConverterForDictionary(Type type)
  {
    var general = type.MakeGeneral();
    if (general == typeof(Dictionary<,>))
    {
      return new DictionaryConverter();
    }
    if (type.GetCustomAttribute<RonMapAttribute>() is RonMapAttribute)
    {
      return new DictionaryConverter();
    }
    return null;
  }

  public static TypeSerializerConverter CreateTypeConverterForTypeWithFields(
    Type type,
    string? typeName
  )
  {
    List<TypeSerializerFieldConverter> list = new List<TypeSerializerFieldConverter>();
    foreach (
      var field in type.GetFieldsAndProperties((BindingFlags)(-1)).Where(x => x.IsRonMember())
    )
    {
      list.Add(
        new MemberInfoSerializer(
          field,
          MemberInfoSerializer.GetFieldConverterForFieldType(field.MemberValueType())
        )
      );
    }

    if (list.Count == 0)
    {
      return new TupleUnitConverter(type.Name);
    }

    return new ObjectSerializerConverter(typeName ?? type.Name, list.ToArray());
  }

  public static TypeSerializerConverter CreateTypeConverterForObjectType(Type type)
  {
    // Create converter from attribute.
    if (CreateTypeConverterFromTypeAttrs(type) is TypeSerializerConverter attrConverter)
    {
      return attrConverter;
    }
    if (CreateTypeConverterForList(type) is TypeSerializerConverter listConverter)
    {
      return listConverter;
    }
    if (CreateTypeConverterForDictionary(type) is TypeSerializerConverter dictConverter)
    {
      return dictConverter;
    }
    return CreateTypeConverterForTypeWithFields(type, null);
  }
}
