namespace RonCS.AST;

/// <summary>
/// Raw string literal.
/// </summary>
/// <param name="source"></param>
public class RonStringRawLit(string source) : StringContent
{
  /// <summary>
  /// The string source code.
  /// </summary>
  public string source = source;

  /// <inheritdoc/>
  public string EvaluateString()
  {
    return source;
  }

  /// <inheritdoc/>
  public string Serialize()
  {
    return source;
  }
}
