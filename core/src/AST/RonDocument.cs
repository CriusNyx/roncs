namespace RonCS.AST;

/// <summary>
/// AST element for a ron document.
/// </summary>
/// <param name="value"></param>
[Serializable]
public class RonDocument(RonElement? value) : RonElement
{
  public RonElement? Value = value;

  public override string RonPrint(RonPrintOptions options)
  {
    return Value?.RonPrint(options) ?? "";
  }
}
