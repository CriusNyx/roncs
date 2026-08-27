using CriusNyx.Util;

namespace RonCS.AST;

/// <summary>
/// AST element for a RonBool
/// </summary>
/// <param name="value"></param>
[Serializable]
[DebugPrint]
public class RonBool(bool value = false) : RonElement
{
  [DebugField]
  public bool Value = value;

  public override string RonPrint(RonPrintOptions options)
  {
    return Value.ToString().ToLower();
  }
}
