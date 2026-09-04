namespace RonCS.AST;

/// <summary>
/// AST element for a float exponent.
/// </summary>
/// <param name="e"></param>
/// <param name="sign"></param>
/// <param name="digits"></param>
public class RonExponent(char? e, char? sign, string? digits)
{
  /// <summary>
  /// Exponent character if provided
  /// </summary>
  public char? e = e;

  /// <summary>
  /// Sign.
  /// </summary>
  public char? sign = sign;

  /// <summary>
  /// Digits
  /// </summary>
  public string? digits = digits;

  /// <summary>
  /// Get a value string representing the number in C#.
  /// </summary>
  /// <returns></returns>
  public string ValueString()
  {
    return $"{e}{sign}{digits}";
  }

  /// <summary>
  /// Convert the element to a Ron string.
  /// </summary>
  /// <returns></returns>
  public string Serialize()
  {
    return $"e{sign}{digits}";
  }
}
