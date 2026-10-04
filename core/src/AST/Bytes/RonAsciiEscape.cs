using RonCS.Exceptions;

namespace RonCS.AST;

/// <summary>
/// AST element for an ascii escape character.
/// </summary>
public class RonAsciiEscape : StringContent, INumberValue
{
  /// <summary>
  /// Create new RonAsciiEscape
  /// </summary>
  /// <param name="source">Source character to be scaped.</param>
  public RonAsciiEscape(char source)
  {
    this.source = source;
  }

  /// <summary>
  /// The source code for the ascii character.
  /// </summary>
  public char? source;

  Type? INumberValue.CSType()
  {
    return typeof(byte);
  }

  /// <inheritdoc/>
  public object EvaluateNumber(Type? hint)
  {
    if (source is char c)
    {
      return (byte)c;
    }
    throw new NullReferenceException("source is null.");
  }

  /// <inheritdoc/>
  public string EvaluateString()
  {
    return source switch
    {
      '\'' => "'",
      '"' => "\"",
      '\\' => "\\",
      'n' => "\n",
      'r' => "\r",
      't' => "\t",
      '0' => "\0",
      _ => throw RonException.CreateNotImplemented(nameof(EvaluateString)),
    };
  }

  /// <inheritdoc/>
  public string Serialize()
  {
    return $"\\{source}";
  }

  /// <inheritdoc/>
  public string ValueString()
  {
    throw RonException.CreateNotImplemented(nameof(ValueString));
  }
}
