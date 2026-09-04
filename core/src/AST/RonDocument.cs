namespace RonCS.AST;

/// <summary>
/// AST element for a ron document.
/// </summary>
/// <param name="value"></param>
[Serializable]
public class RonDocument(RonElement? value) : RonElement
{
  /// <summary>
  /// The value of the document body
  /// </summary>
  public RonElement? Value = value;

  /// <inheritdoc/>
  public override string RonPrint(RonPrintOptions options)
  {
    return Value?.RonPrint(options) ?? "";
  }
}
