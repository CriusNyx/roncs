using RonCS.AST;
using RonCS.Exceptions;
using Superpower;
using Superpower.Model;

namespace RonCS;

public static partial class Ron
{
  static SerializationContext globalContext = SerializationContext.CreateGlobalContext();

  /// <summary>
  /// Try to tokenize the input.
  /// If successful returns the result.
  /// Otherwise returns the exception.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="result"></param>
  /// <param name="exception"></param>
  /// <returns></returns>
  public static bool TryTokenize(
    string source,
    out TokenList<RonTokenKind> result,
    out Exception exception
  )
  {
    return RonLexer.TryTokenize(source, out result, out exception);
  }

  /// <summary>
  /// Parse the source and return the resulting ron document. Throw an exception if not successful.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static RonDocument Parse(string source)
  {
    if (TryParse(source, out var doc, out var exception))
    {
      return doc;
    }
    else
    {
      throw exception;
    }
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

    if (!TryTokenize(source, out var tokenList, out exception))
    {
      return false;
    }

    var result = RonParser.Ron.Select(x => x.AsNotNull<RonDocument>()).TryParse(tokenList);

    if (result.HasValue)
    {
      document = result.Value;
      return true;
    }
    else
    {
      exception = new RonParseException(result);
      return false;
    }
  }

  /// <summary>
  /// Deserialize the input, or throw an exception if failed.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="hint"></param>
  /// <returns></returns>
  public static object? Deserialize(string source, Type? hint)
  {
    if (TryDeserialize(source, hint, out var o, out var exception))
    {
      return o;
    }
    throw exception;
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

    if (!TryParse(source, out var doc, out exception))
    {
      return false;
    }

    var result = globalContext.DeserializeElement(doc, typeHint, "");
    if (result.isSuccess)
    {
      output = result.value;
      return true;
    }
    exception = result.exception;
    return false;
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
