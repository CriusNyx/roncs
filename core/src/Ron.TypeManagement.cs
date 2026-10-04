using RonCS.Converters;

namespace RonCS;

public static partial class Ron
{
  /// <summary>
  /// Register a type that ron knows how to deserialize.
  /// When Ron cannot infer a type from context it will check the type cache for a registered type.
  /// </summary>
  /// <param name="types">The type to be known by Ron.</param>
  public static void RegisterType(params Type[] types)
  {
    foreach (var type in types)
    {
      globalContext.RegisterType(type);
    }
  }

  /// <summary>
  /// Register a serialization converter for a known type.
  /// </summary>
  /// <param name="type">The type that this converter applies to.</param>
  /// <param name="converter">The converter for this type.</param>
  public static void RegisterTypeConverter(Type type, TypeSerializerConverter converter)
  {
    globalContext.RegisterTypeConverter(type, converter);
  }

  /// <summary>
  /// Register a type to serialize as a list.
  /// </summary>
  /// <param name="type">The type to serialize and deserialize as a list.</param>
  public static void RegisterListType(Type type)
  {
    globalContext.RegisterListType(type);
  }

  /// <summary>
  /// Register a type to serialize as a dictionary.
  /// </summary>
  /// <param name="type">The type to serialize and deserialize as a dictionary.</param>
  public static void RegisterDictionaryType(Type type)
  {
    globalContext.RegisterMapType(type);
  }

  /// <summary>
  /// Register custom converter to serialize T as a tuple.
  /// </summary>
  /// <typeparam name="T">The type to register the custom converter for.</typeparam>
  /// <param name="converter">The function to convert the element T to a tuple.</param>
  public static void RegisterTupleConverter<T>(Func<T, object[]> converter)
  {
    globalContext.RegisterTupleConverter(converter);
  }

  /// <summary>
  /// Register a proxy type for another type.
  /// </summary>
  /// <param name="sourceType">The source type to be converted to and from a proxy.</param>
  /// <param name="proxyType">The proxy type to be serialized as Ron.</param>
  public static void RegisterProxyType(Type sourceType, Type proxyType)
  {
    globalContext.RegisterProxyType(sourceType, proxyType);
  }

  /// <summary>
  /// Clear all settings in the global context.
  /// </summary>
  public static void ResetGlobalContext()
  {
    globalContext = SerializationContext.CreateGlobalContext();
  }
}
