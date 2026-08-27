using CriusNyx.Util;

namespace RonCS.AST;

/// <summary>
/// AST element representing a ron none.
/// </summary>
[DebugPrint]
[Serializable]
public class RonNone : RonElement
{
  public override string RonPrint(RonPrintOptions options)
  {
    return "None";
  }
}
