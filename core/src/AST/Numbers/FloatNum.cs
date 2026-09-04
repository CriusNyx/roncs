namespace RonCS.AST;

/// <summary>
/// Base class for differentiating standard and special float numbers.
/// </summary>
public abstract class RonFloatNumber
{
  /// <summary>
  /// Get a string representing the value of the element.
  /// </summary>
  /// <returns></returns>
  public abstract string ValueString();

  /// <summary>
  /// Convert the element to a Ron string.
  /// </summary>
  /// <returns></returns>
  public abstract string Serialize();
};
