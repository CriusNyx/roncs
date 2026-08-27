using CriusNyx.Util;

namespace RonCS.AST;

/// <summary>
/// AST element for a ron some.
/// </summary>
/// <param name="value"></param>
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
