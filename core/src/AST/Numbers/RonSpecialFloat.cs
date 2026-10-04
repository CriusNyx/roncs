namespace RonCS.AST;

/// <summary>
/// Type of a special float.
/// </summary>
public enum SpecialFloatType
{
  /// <summary>
  /// Infinity
  /// </summary>
  inf,

  /// <summary>
  /// Not a number
  /// </summary>
  NaN,
}

/// <summary>
/// AST element for a special float number.
/// </summary>
public class RonSpecialFloat : RonFloatNumber
{
  /// <summary>
  /// The type of the float.
  /// </summary>
  public SpecialFloatType? type;

  /// <summary>
  /// Create a new special float.
  /// </summary>
  /// <param name="type">The type of the special float.</param>
  public RonSpecialFloat(SpecialFloatType? type)
  {
    this.type = type;
  }

  /// <inheritdoc/>
  public override string Serialize()
  {
    return type.NotNull(nameof(type)).ToString().NotNull(nameof(type));
  }

  /// <inheritdoc/>
  public override string ValueString()
  {
    return type?.ToString() ?? "";
  }
}
