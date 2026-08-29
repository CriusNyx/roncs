namespace RonCS.AST;

/// <summary>
/// AST element for a ron list.
/// </summary>
/// <param name="values"></param>
public class RonList(params RonElement[]? values) : RonElement
{
  /// <summary>
  /// The values in the ron list.
  /// </summary>
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
