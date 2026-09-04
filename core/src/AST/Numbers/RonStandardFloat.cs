namespace RonCS.AST;

/// <summary>
/// AST element representing a standard float number.
/// </summary>
/// <param name="digits"></param>
/// <param name="exponent"></param>
public class RonStandardFloat(string? digits, RonExponent? exponent) : RonFloatNumber
{
  /// <summary>
  /// The digits of the float, if provided.
  /// </summary>
  public string? digits = digits;

  /// <summary>
  /// The exponent of the float, if provided.
  /// </summary>
  public RonExponent? exponent = exponent;

  /// <inheritdoc/>
  public override string ValueString()
  {
    return digits + exponent?.ValueString();
  }

  /// <inheritdoc/>
  public override string Serialize()
  {
    return $"{digits}{exponent?.Serialize()}";
  }
}
