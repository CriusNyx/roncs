using RonCS.AST;
using RonCS.Exceptions;
using Superpower;
using Superpower.Model;

namespace RonCS;

/// <summary>
/// Ron Serialization/Deserialization API.
/// </summary>
public static partial class Ron
{
  static SerializationContext globalContext = SerializationContext.CreateGlobalContext();

  /// <summary>
  /// Try to tokenize the input.
  /// If successful returns the result.
  /// Otherwise returns the exception.
  /// </summary>
  /// <param name="source">The ron string to tokenize.</param>
  /// <param name="result">The result if successful.</param>
  /// <param name="exception">The exception if tokenization fails.</param>
  /// <returns>Bool indicating if the tokenization was successful.</returns>
  public static bool TryTokenize(
    string source,
    out TokenList<RonTokenKind> result,
    out Exception exception
  )
  {
    return RonLexer.TryTokenize(source, out result, out exception);
  }

  /// <summary>
  /// Tokenize the input and return the token list.
  /// </summary>
  /// <param name="source">The ron string to tokenize.</param>
  /// <returns>The token list.</returns>
  public static TokenList<RonTokenKind> Tokenize(string source)
  {
    if (TryTokenize(source, out var result, out var exception))
    {
      return result;
    }
    throw exception;
  }

  /// <summary>
  /// Parse the source and return the resulting ron document. Throw an exception if not successful.
  /// </summary>
  /// <param name="source">The ron string to parse.</param>
  /// <returns>AST tree for the ron string.</returns>
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
  /// <param name="source">The ron string to parse.</param>
  /// <param name="document">AST for the ron string if successful.</param>
  /// <returns>Bool indicating if parsing was successful.</returns>
  public static bool TryParse(string source, out RonDocument document)
  {
    return TryParse(source, out document, out var _);
  }

  /// <summary>
  /// Try to parse the source, returning the document if successful, or the exception if not successful.
  /// </summary>
  /// <param name="source">The source document to parse.</param>
  /// <param name="document">The document if successful.</param>
  /// <param name="exception">The exception thrown if not successful.</param>
  /// <returns>Bool indicating if parsing was successful.</returns>
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
  /// <param name="source">The Ron string to deserialize.</param>
  /// <param name="hint">Type hint for type inference.</param>
  /// <returns>The deserialized object.</returns>
  public static object? Deserialize(string source, Type? hint)
  {
    if (TryDeserialize(source, hint, out var o, out var exception))
    {
      return o;
    }
    throw exception;
  }

  /// <summary>
  /// Deserialize the Ron file as type T.
  /// </summary>
  /// <typeparam name="T">The type to deserialize as.</typeparam>
  /// <param name="source">The ron source code.</param>
  /// <returns>The value of T if successful.</returns>
  public static T Deserialize<T>(string source)
  {
    if (TryDeserialize(source, typeof(T), out var o, out var e))
    {
      if (o is T t)
      {
        return t;
      }
      return default!;
    }

    throw e;
  }

  /// <summary>
  /// Try to deserialize the result
  /// </summary>
  /// <param name="source">The Ron string to deserialize.</param>
  /// <param name="typeHint">The hint for type inference.</param>
  /// <param name="output">The deserialized object if successful.</param>
  /// <returns>Bool indicating if deserialization was successful.</returns>
  public static bool TryDeserialize(string source, Type? typeHint, out object? output)
  {
    return TryDeserialize(source, typeHint, out output, out var _);
  }

  /// <summary>
  /// Try to deserialize the input, returning an object if successful, or an exception if failed.
  /// </summary>
  /// <param name="source">The Ron string to deserialize.</param>
  /// <param name="typeHint">The hint for type inference.</param>
  /// <param name="output">The deserialized output if successful.</param>
  /// <param name="exception">The exception if serialization was not successful.</param>
  /// <returns>Bool indicating if deserialization was successful.</returns>
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
  /// <param name="source">The Ron string to serialize.</param>
  /// <param name="options">Serialization options.</param>
  /// <returns>The ron string.</returns>
  public static string Serialize(object source, RonPrintOptions options = null!)
  {
    options = options ?? RonPrintOptions.Compact();
    return globalContext.ToAST(source).RonPrint(options);
  }
}
