using RonCS.Converters;

namespace RonCS;

public partial class SerializationContext
{
  /// <summary>
  /// Register a new known type
  /// </summary>
  /// <param name="type"></param>
  public void RegisterType(Type type)
  {
    RonTypes.Add(type);
  }

  /// <summary>
  /// Register a custom type converter for the type.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="converter"></param>
  public void RegisterTypeConverter(Type type, TypeSerializerConverter converter)
  {
    converterCache[type] = converter;
  }

  /// <summary>
  /// Register type to be serialized as a list.
  /// </summary>
  /// <param name="type"></param>
  public void RegisterListType(Type type)
  {
    converterCache[type] = new ListConverter();
  }

  /// <summary>
  /// Register type to be serialized as a map.
  /// </summary>
  /// <param name="type"></param>
  public void RegisterMapType(Type type)
  {
    converterCache[type] = new DictionaryConverter();
  }

  /// <summary>
  /// Register converter to serialize a type as a tuple.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="converter"></param>
  public void RegisterTupleConverter<T>(Func<T, object[]> converter)
  {
    RegisterTypeConverter(
      typeof(T),
      new TupleFunctionConverter(typeof(T).Name, (o) => converter((T)o))
    );
  }

  /// <summary>
  /// Register a custom proxy for the source type.
  /// </summary>
  /// <param name="sourceType"></param>
  /// <param name="proxyType"></param>
  public void RegisterProxyType(Type sourceType, Type proxyType)
  {
    RegisterTypeConverter(
      sourceType,
      new ProxyConverter(
        proxyType,
        TypeSerializerConverter.CreateTypeConverterForTypeWithFields(proxyType, sourceType.Name)
      )
    );

    proxyTypes[sourceType] = proxyType;
  }
}
