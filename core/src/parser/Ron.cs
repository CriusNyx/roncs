using CriusNyx.Util;
using Superpower;
using Superpower.Model;

namespace RonCS;

public static partial class Ron
{
  static SerializationContext globalContext = SerializationContext.CreateGlobalContext();

  public static TokenList<RonTokenKind> Tokenize(string source)
  {
    return RonLexer.Tokenize(source);
  }

  public static RonDocument Parse(TokenList<RonTokenKind> tokenList)
  {
    return RonParser.Ron.Parse(tokenList).AsNotNull<RonDocument>("Parsed");
  }

  public static RonDocument Parse(string source)
  {
    return Parse(Tokenize(source));
  }

  public static object Deserialize(string source, Type? hint)
  {
    var document = Parse(source);
    var deserialized = Deserialize(document, hint);
    return deserialized;
  }

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
