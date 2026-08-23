using CriusNyx.Results;
using CriusNyx.Util;
using RonCS.AST;
using Superpower;
using Superpower.Model;

namespace RonCS;

public static partial class Ron
{
  static SerializationContext globalContext = SerializationContext.CreateGlobalContext();

  /// <summary>
  /// Tokenize the input, returning a result.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static Result<TokenList<RonTokenKind>, Exception> TokenizeResult(string source)
  {
    return RonLexer.TokenizeResult(source);
  }

  /// <summary>
  /// Parse the input, returning a result.
  /// </summary>
  /// <param name="tokenResult"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public static Result<RonDocument, Exception> ParseResult(
    Result<TokenList<RonTokenKind>, Exception> tokenResult
  )
  {
    return tokenResult.AndThen(tokenList =>
    {
      return RonParser
        .Ron.Select(x => x.AsNotNull<RonDocument>())
        .TryParse(tokenList)
        .FromParseResult();
    });
  }

  /// <summary>
  /// Parse the input, returning a result.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static Result<RonDocument, Exception> ParseResult(string source)
  {
    return ParseResult(TokenizeResult(source));
  }

  /// <summary>
  /// Parse the source and return the resulting ron document. Throw an exception if not successful.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static RonDocument Parse(string source)
  {
    return ParseResult(source)
      .UnwrapOrElse(x =>
      {
        throw x;
      });
  }

  /// <summary>
  /// Try to parse the source, returning the document if successful.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="document"></param>
  /// <returns></returns>
  public static bool TryParse(string source, out RonDocument document)
  {
    return TryParse(source, out document, out var _);
  }

  /// <summary>
  /// Try to parse the source, returning the document if successful, or the exception if not successful.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="document"></param>
  /// <returns></returns>
  public static bool TryParse(string source, out RonDocument document, out Exception exception)
  {
    document = null!;
    exception = null!;
    var result = ParseResult(source);
    if (result.IsOk())
    {
      document = result.Unwrap();
      return true;
    }
    exception = result.UnwrapErr();
    return false;
  }

  /// <summary>
  /// Deserialize the source as a result.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="hint"></param>
  /// <returns></returns>
  public static Result<object?, Exception> DeserializeResult(string source, Type? hint)
  {
    return DeserializeResult(ParseResult(source), hint);
  }

  /// <summary>
  /// Deserialize the input, or throw an exception if failed.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="hint"></param>
  /// <returns></returns>
  public static object? Deserialize(string source, Type? hint)
  {
    return DeserializeResult(source, hint).Unwrap();
  }

  /// <summary>
  /// Try to deserialize the result
  /// </summary>
  /// <param name="source"></param>
  /// <param name="typeHint"></param>
  /// <param name="output"></param>
  /// <returns></returns>
  public static bool TryDeserialize(string source, Type? typeHint, out object? output)
  {
    return TryDeserialize(source, typeHint, out output, out var _);
  }

  /// <summary>
  /// Try to deserialize the input, returning an object if successful, or an exception if failed.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="typeHint"></param>
  /// <param name="output"></param>
  /// <param name="exception"></param>
  /// <returns></returns>
  public static bool TryDeserialize(
    string source,
    Type? typeHint,
    out object? output,
    out Exception exception
  )
  {
    output = null;
    exception = null!;
    var result = DeserializeResult(source, typeHint);
    if (result.IsOk())
    {
      output = result.Unwrap();
      return true;
    }
    exception = result.UnwrapErr();
    return false;
  }

  /// <summary>
  /// Deserialize the ron element.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="typeHint"></param>
  /// <returns></returns>
  public static Result<object?, Exception> DeserializeResult(
    Result<RonDocument, Exception> parseResult,
    Type? typeHint
  )
  {
    return parseResult.AndThen(doc => globalContext.DeserializeElement(doc, typeHint, ""));
  }

  /// <summary>
  /// Convert an object to a ron string.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string Serialize(object source, RonPrintOptions options = null!)
  {
    options = options ?? RonPrintOptions.Compact();
    return globalContext.ToAST(source).RonPrint(options);
  }
}
