namespace RonCS.AST;

/// <summary>
/// Represents part of a string.
/// </summary>
public interface StringContent
{
  /// <summary>
  /// Evaluate the element as a C# string.
  /// </summary>
  /// <returns></returns>
  string EvaluateString();

  /// <summary>
  /// Convert the element to a RON string.
  /// </summary>
  /// <returns></returns>
  string Serialize();
}
