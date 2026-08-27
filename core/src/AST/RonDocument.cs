using CriusNyx.Util;

namespace RonCS.AST;

/// <summary>
/// AST element for a ron document.
/// </summary>
/// <param name="value"></param>
[DebugPrint]
[Serializable]
public class RonDocument(RonElement? value) : RonElement
{
  [DebugField]
  public RonElement? Value = value;

  public override string RonPrint(RonPrintOptions options)
  {
    return Value?.RonPrint(options) ?? "";
  }
}
