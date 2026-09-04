using RonCS.Exceptions;

namespace RonCS.AST;

/// <summary>
/// AST element for a Ron map
/// </summary>
/// <param name="values"></param>
[Serializable]
public class RonMap(params RonElement[]? values) : RonElement
{
  /// <summary>
  /// Values in the ron map.
  /// If these are correct they should be map items.
  /// </summary>
  public RonElement[]? Values = values;

  /// <inheritdoc/>
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
