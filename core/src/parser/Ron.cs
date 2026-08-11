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
  /// Register a serialization converter for a known type.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="converter"></param>
  public static void RegisterTypeConverter(Type type, TypeSerializerConverter converter)
  {
    globalContext.RegisterTypeConverter(type, converter);
  }

  /// <summary>
  /// Register a type as a list.
  /// </summary>
  /// <param name="type"></param>
  public static void RegisterListType(Type type)
  {
    globalContext.RegisterListType(type);
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

  public static string Serialize(object source, RonPrintOptions options = null!)
  {
    options = options ?? RonPrintOptions.Compact();
    return globalContext.ToAST(source).RonPrint(options);
  }
}
