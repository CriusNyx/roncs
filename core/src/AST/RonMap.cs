using CriusNyx.Util;

namespace RonCS.AST;

/// <summary>
/// AST element for a Ron map
/// </summary>
/// <param name="values"></param>
[DebugPrint]
[Serializable]
public class RonMap(params RonElement[]? values) : RonElement
{
  /// <summary>
  /// Values in the ron map.
  /// If these are correct they should be map items.
  /// </summary>
  [DebugField]
  public RonElement[]? Values = values;

  public override string RonPrint(RonPrintOptions options)
  {
    switch (options.mode)
    {
      case RonPrintMode.Compact:
        return "{" + Values?.Select(x => x.RonPrint(options)).StringJoin(",") + "}";
      case RonPrintMode.Pretty:
        return "{\n"
          + Values?.Select(x => x.RonPrint(options)).StringJoin(",\n").Indent(options.indent)
          + "\n}";
      default:
        throw options.mode.AsEnumException();
    }
  }
}
