namespace RonCS.AST;

/// <summary>
/// Raw string.
/// </summary>
/// <param name="content"></param>
public class RonStringRawContent(StringContent content) : StringContent
{
  /// <summary>
  /// The content of this raw string.
  /// </summary>
  public StringContent content = content;

  /// <inheritdoc/>
  public string EvaluateString()
  {
    return content.EvaluateString();
  }

  /// <inheritdoc/>
  public string Serialize()
  {
    return content.Serialize();
  }
}
