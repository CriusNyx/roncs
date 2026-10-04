namespace RonCS.AST;

/// <summary>
/// AST element for an ascii literal character.
/// </summary>
public class RonAsciiLiteral : INumberValue
{
  char c;

  /// <summary>
  /// Create new RonAsciiLiteral
  /// </summary>
  /// <param name="c">The character represented by the literal.</param>
  public RonAsciiLiteral(char c)
  {
    this.c = c;
  }

  Type? INumberValue.CSType()
  {
    return typeof(byte);
  }

  /// <inheritdoc/>
  public object EvaluateNumber(Type? hint)
  {
    return (byte)c;
  }

  /// <inheritdoc/>
  public string ValueString()
  {
    return c.ToString();
  }
}
