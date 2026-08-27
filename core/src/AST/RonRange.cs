using CriusNyx.Util;

namespace RonCS.AST;

/// <summary>
/// The ron element operator.
/// </summary>
public enum RonRangeOperator
{
  Exclusive,
  Inclusive,
}

/// <summary>
/// AST element for a Ron range.
/// </summary>
/// <param name="lower"></param>
/// <param name="op"></param>
/// <param name="upper"></param>
[DebugPrint]
public class RonRange(
  RonElement? lower = null,
  RonRangeOperator? op = null,
  RonElement? upper = null
) : RonElement
{
  /// <summary>
  /// The lower value of the range.
  /// </summary>
  [DebugField]
  public RonElement? lower = lower;

  /// <summary>
  /// The operator of the range.
  /// </summary>
  [DebugField]
  public RonRangeOperator? op = op;

  /// <summary>
  /// The upper value of the range.
  /// </summary>
  [DebugField]
  public RonElement? upper = upper;

  public override string RonPrint(RonPrintOptions options)
  {
    return lower?.RonPrint(options) + op + upper?.RonPrint(options);
  }
}
