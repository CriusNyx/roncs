namespace RonCS;

public class RonSerializer
{
  static SerializationContext globalContext = SerializationContext.CreateGlobalContext();

  /// <summary>
  /// Register a type that ron knows how to deserialize.
  /// When Ron cannot infer a type from context it will check the type cache for a registered type.
  /// </summary>
  /// <param name="types"></param>
  public static void RegisterTypes(params Type[] types)
  {
    foreach (var type in types)
    {
      globalContext.RegisterType(type);
    }
  }

  /// <summary>
  /// Deserialize the ron element.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="typeHint"></param>
  /// <returns></returns>
  public static object Deserialize(RonElement element, Type? typeHint)
  {
    return globalContext.DeserializeElement(element, typeHint);
  }
}
