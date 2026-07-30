namespace RonCS;

internal class PrimitiveDeserializers
{
  internal static object DeserializeNumber(RonElement element, Type? typeHint)
  {
    if (element is NumberValue numVal)
    {
      return numVal.Parse(typeHint);
    }
    throw new NotImplementedException();
  }

  internal static object DeserializeString(RonElement ronElement)
  {
    if (ronElement is StringValue stringValue)
    {
      return stringValue.Evaluate();
    }
    throw new NotImplementedException();
  }
}

public class RonSerializer
{
  private static readonly IReadOnlyDictionary<Type, Func<RonElement, object>> PrimitiveTypes =
    new Dictionary<Type, Func<RonElement, object>>()
    {
      { typeof(string), PrimitiveDeserializers.DeserializeString },
    };

  static SerializationContext globalContext = SerializationContext.CreateGlobalContext();

  public static void RegisterTypes(params Type[] types)
  {
    foreach (var type in types)
    {
      globalContext.RegisterType(type);
    }
  }

  public static object Deserialize(RonElement element, Type? typeHint)
  {
    return globalContext.DeserializeElement(element, typeHint);
  }
}
