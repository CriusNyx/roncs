using System.Collections;
using System.Reflection;
using CriusNyx.Util;
using RonCS;

public interface TypeSerializerConverter
{
  public RonElement ToAST(SerializationContext context, object source);

  public static TypeSerializerConverter? CreateTypeConverterFromTypeAttrs(Type type)
  {
    foreach (var attr in type.GetCustomAttributes())
    {
      if (attr is RonListAttribute)
      {
        return new ListConverter();
      }
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

  public static TypeSerializerConverter CreateTypeConverterForTypeWithFields(Type type)
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

    return new ObjectSerializerConverter(type.Name, list.ToArray());
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
    return CreateTypeConverterForTypeWithFields(type);
  }
}

public interface TypeSerializerFieldConverter
{
  public RonElement FieldElementForObject(SerializationContext context, object source);
}

public class TupleUnitConverter(string name) : TypeSerializerConverter
{
  public RonElement ToAST(SerializationContext context, object source)
  {
    return new RonUnitStruct(new RonIdentifier(name));
  }
}

public class ObjectSerializerConverter(string objectName, TypeSerializerFieldConverter[] converters)
  : TypeSerializerConverter
{
  public RonElement ToAST(SerializationContext context, object source)
  {
    var fields = converters.Select(x => x.FieldElementForObject(context, source)).ToArray();
    return new RonNamedValueStruct(new RonIdentifier(objectName), fields);
  }
}

public class FieldInfoSerializer(FieldInfo field, TypeSerializerConverter? converter)
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

public class ListConverter : TypeSerializerConverter
{
  public RonElement ToAST(SerializationContext context, object source)
  {
    var list = source.AsNotNull<IList>(nameof(source)).Cast<object>();
    var elements = list.Select(element => context.ToAST(element));
    return new RonList(elements.ToArray());
  }
}

public class DictionaryConverter : TypeSerializerConverter
{
  public RonElement ToAST(SerializationContext context, object source)
  {
    var elements = new List<RonElement>();
    var dict = source.AsNotNull<IDictionary>(nameof(source));
    foreach (var key in dict.Keys)
    {
      var value = dict[key];
      elements.Add(
        new RonMapItem(
          new StringValue(new StringLit(key.ToString().NotNull())),
          context.ToAST(value)
        )
      );
    }
    return new RonMap(elements.ToArray());
  }
}
