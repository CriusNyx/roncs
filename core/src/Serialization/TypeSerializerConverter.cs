using System.Collections;
using System.Reflection;

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
    foreach (var field in type.GetFields().Where(x => x.IsPublic && !x.IsStatic))
    {
      list.Add(
        new FieldInfoSerializer(
          field,
          FieldInfoSerializer.GetFieldConverterForFieldType(field.FieldType)
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
