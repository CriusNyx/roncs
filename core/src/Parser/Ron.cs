using CriusNyx.Results;
using CriusNyx.Util;
using Superpower;
using Superpower.Model;

namespace RonCS;

public static partial class Ron
{
  static SerializationContext globalContext = SerializationContext.CreateGlobalContext();

  public static Result<TokenList<RonTokenKind>, Exception> TokenizeResult(string source)
  {
    return RonLexer.Tokenize(source);
  }

  public static Result<RonDocument, Exception> ParseResult(
    Result<TokenList<RonTokenKind>, Exception> tokenResult
  )
  {
    return tokenResult.AndThen(tokenList =>
      RonParser
        .Ron.Select(x => x.AsNotNull<RonDocument>())
        .TryParse(tokenList)
        .IntoResult((x) => throw new NotImplementedException())
    );
  }

  public static Result<RonDocument, Exception> ParseResult(string source)
  {
    return ParseResult(TokenizeResult(source));
  }

  public static RonDocument Parse(string source)
  {
    return ParseResult(source).Unwrap();
  }

  public static bool TryParse(string source, out RonDocument document)
  {
    var result = ParseResult(source);
    if (result.IsOk())
    {
      document = result.Unwrap();
      return true;
    }
    document = null!;
    return false;
  }

  public static Result<object, Exception> DeserializeResult(string source, Type? hint)
  {
    return DeserializeResult(ParseResult(source), hint);
  }

  public static object Deserialize(string source, Type? hint)
  {
    return DeserializeResult(source, hint).Unwrap();
  }

  public static bool TryDeserialize(string source, Type? hint, out object output)
  {
    var result = DeserializeResult(source, hint);
    if (result.IsOk())
    {
      output = result.Unwrap();
      return true;
    }
    output = null!;
    return false;
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
  /// Register a type to serialize as a list.
  /// </summary>
  /// <param name="type"></param>
  public static void RegisterListType(Type type)
  {
    globalContext.RegisterListType(type);
  }

  /// <summary>
  /// Register a type to serialize as a dictionary.
  /// </summary>
  /// <param name="type"></param>
  public static void RegisterDictionaryType(Type type)
  {
    globalContext.RegisterDictionaryType(type);
  }

  /// <summary>
  /// Deserialize the ron element.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="typeHint"></param>
  /// <returns></returns>
  public static Result<object, Exception> DeserializeResult(
    Result<RonDocument, Exception> parseResult,
    Type? typeHint
  )
  {
    return parseResult.Map(doc => globalContext.DeserializeElement(doc, typeHint));
  }

  public static string Serialize(object source, RonPrintOptions options = null!)
  {
    options = options ?? RonPrintOptions.Compact();
    return globalContext.ToAST(source).RonPrint(options);
  }
}
