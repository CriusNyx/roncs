using RonCS.Exceptions;

namespace RonCS.AST;

/// <summary>
/// AST element for a byte escape character.
/// </summary>
/// <param name="left"></param>
/// <param name="right"></param>
public class RonByteEscape(char left, char right) : StringContent, INumberValue
{
  /// <summary>
  /// The left portion of the byte.
  /// </summary>
  public char? left = left;

  /// <summary>
  /// The right portion of the byte.
  /// </summary>
  public char? right = right;

  Type? INumberValue.CSType()
  {
    return typeof(byte);
  }

  /// <inheritdoc/>
  public object EvaluateNumber(Type? hint)
  {
    return byte.Parse($"{left}{right}", System.Globalization.NumberStyles.HexNumber);
  }

  /// <inheritdoc/>
  public string EvaluateString()
  {
    char l = (char)left.NotNull(nameof(left))!;
    char r = (char)right.NotNull(nameof(right))!;
    var b = byte.Parse([l, r], System.Globalization.NumberStyles.HexNumber);
    return ((char)b).ToString();
  }

  /// <inheritdoc/>
  public string Serialize()
  {
    return $"x{left}{right}";
  }

  /// <inheritdoc/>
  public string ValueString()
  {
    throw RonException.CreateNotImplemented(nameof(ValueString));
  }
}
