using CriusNyx.Util;

namespace RonCS.AST;

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
