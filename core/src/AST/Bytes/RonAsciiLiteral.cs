namespace RonCS.AST;

/// <summary>
/// AST element for an ascii literal character.
/// </summary>
/// <param name="c"></param>
public class RonAsciiLiteral(char c) : INumberValue
{
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
