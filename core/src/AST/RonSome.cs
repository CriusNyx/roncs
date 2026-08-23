using CriusNyx.Util;

namespace RonCS.AST;

[DebugPrint]
[Serializable]
public class RonSome(RonElement? value = null) : RonElement
{
  [DebugField]
  public RonElement? value = value;

  public override string RonPrint(RonPrintOptions options)
  {
    return $"Some({value?.RonPrint(options)})";
  }
}
