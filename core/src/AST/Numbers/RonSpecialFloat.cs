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
/// <param name="type"></param>
public class RonSpecialFloat(SpecialFloatType? type) : RonFloatNumber
{
  /// <summary>
  /// The type of the float.
  /// </summary>
  public SpecialFloatType? type = type;

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
