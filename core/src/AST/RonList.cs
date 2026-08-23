using CriusNyx.Util;

namespace RonCS.AST;

[DebugPrint]
public class RonList(params RonElement[]? values) : RonElement
{
  [DebugField]
  public readonly RonElement[]? Values = values;

  public override string RonPrint(RonPrintOptions options)
  {
    switch (options.mode)
    {
      case RonPrintMode.Compact:
        return "[" + Values?.Select(x => x.RonPrint(options)).StringJoin(",") + "]";
      case RonPrintMode.Pretty:
        return "[\n"
          + Values?.Select(x => x.RonPrint(options)).StringJoin(",\n").Indent("  ")
          + "\n]";

      default:
        throw options.mode.AsEnumException();
    }
  }
}
