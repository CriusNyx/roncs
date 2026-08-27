using CriusNyx.Util;

namespace RonCS.AST;

/// <summary>
/// AST element for a ron named value. These are typically body elements for a ron struct.
/// </summary>
/// <param name="name"></param>
/// <param name="value"></param>
[DebugPrint]
[Serializable]
public class RonNamedValue(RonElement? name = null, RonElement? value = null) : RonElement
{
  /// <summary>
  /// The name of the element. This should be a ron identifier.
  /// </summary>
  [DebugField]
  public RonElement? name = name;

  /// <summary>
  /// The value of the element.
  /// </summary>
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
