namespace RonCS.AST;

/// <summary>
/// AST element for the exponent part of a floating point number.
/// </summary>
public class RonExponent
{
  /// <summary>
  /// Exponent character if provided
  /// </summary>
  public char? e;

  /// <summary>
  /// Sign.
  /// </summary>
  public char? sign;

  /// <summary>
  /// Digits
  /// </summary>
  public string? digits;

  /// <summary>
  /// Create a new RonExponent.
  /// </summary>
  /// <param name="e">The Character used to represent the exponent.</param>
  /// <param name="sign">The sign of the exponent.</param>
  /// <param name="digits">The digits of the exponent.</param>
  public RonExponent(char? e, char? sign, string? digits)
  {
    this.e = e;
    this.sign = sign;
    this.digits = digits;
  }

  /// <summary>
  /// Get a value string representing the number in C#.
  /// </summary>
  /// <returns>A string representing the value of the exponent.</returns>
  public string ValueString()
  {
    return $"{e}{sign}{digits}";
  }

  /// <summary>
  /// Convert the element to a Ron string.
  /// </summary>
  /// <returns>A ron string for the exponent.</returns>
  public string Serialize()
  {
    return $"e{sign}{digits}";
  }
}
