using CriusNyx.Util;

namespace RonCS.AST;

[DebugPrint]
[Serializable]
public class RonNamedValue(RonElement? name = null, RonElement? value = null) : RonElement
{
  [DebugField]
  public RonElement? name = name;

  [DebugField]
  public RonElement? value = value;

  public override string RonPrint(RonPrintOptions options)
  {
    switch (options.mode)
    {
      case RonPrintMode.Compact:
        return name?.RonPrint(options) + ":" + value?.RonPrint(options);
      case RonPrintMode.Pretty:
        return name?.RonPrint(options) + ": " + value?.RonPrint(options);
      default:
        throw options.mode.AsEnumException();
    }
  }
}
