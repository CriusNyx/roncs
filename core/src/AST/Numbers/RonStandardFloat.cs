namespace RonCS.AST;

/// <summary>
/// AST element representing a standard float number.
/// </summary>
public class RonStandardFloat : RonFloatNumber
{
  /// <summary>
  /// The digits of the float, if provided.
  /// </summary>
  public string? digits;

  /// <summary>
  /// The exponent of the float, if provided.
  /// </summary>
  public RonExponent? exponent;

  /// <summary>
  /// Create a new standard floating point number.
  /// </summary>
  /// <param name="digits">The digits of the number.</param>
  /// <param name="exponent">The exponent of the number.</param>
  public RonStandardFloat(string? digits, RonExponent? exponent)
  {
    this.digits = digits;
    this.exponent = exponent;
  }

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
