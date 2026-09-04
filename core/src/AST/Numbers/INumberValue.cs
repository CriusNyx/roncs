namespace RonCS.AST;

/// <summary>
/// Common interface for all number AST nodes.
/// </summary>
public interface INumberValue
{
  /// <summary>
  /// String representing the value of the number.
  /// Used for C# type conversion.
  /// </summary>
  /// <returns></returns>
  public string ValueString();

  /// <summary>
  /// Convert the value to a number.
  /// </summary>
  /// <param name="hint"></param>
  /// <returns></returns>
  public object EvaluateNumber(Type? hint);

  internal Type? CSType();
}
