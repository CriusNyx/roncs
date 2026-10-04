using RonCS.Exceptions;

namespace RonCS.AST;

/// <summary>
/// AST element for a byte escape character.
/// </summary>
public class RonByteEscape : StringContent, INumberValue
{
  /// <summary>
  /// The left hex of the byte.
  /// </summary>
  public char? left;

  /// <summary>
  /// The right hex of the byte.
  /// </summary>
  public char? right;

  /// <summary>
  /// Create a new RonByteEscape.
  /// </summary>
  /// <param name="left">The left hex byte.</param>
  /// <param name="right">The right hex byte.</param>
  public RonByteEscape(char left, char right)
  {
    this.left = left;
    this.right = right;
  }

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
