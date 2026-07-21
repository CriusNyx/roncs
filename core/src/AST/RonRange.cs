using CriusNyx.Util;

public enum RonRangeOperator
{
  Exclusive,
  Inclusive,
}

[DebugPrint]
public class RonRange(
  RonElement? lower = null,
  RonRangeOperator? op = null,
  RonElement? upper = null
) : RonElement
{
  [DebugField]
  public RonElement? lower = lower;

  [DebugField]
  public RonRangeOperator? op = op;

  [DebugField]
  public RonElement? upper = upper;
}
