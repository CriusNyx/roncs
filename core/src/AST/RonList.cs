using CriusNyx.Util;

[DebugPrint]
public class RonList(RonElement[]? values = null) : RonElement
{
  [DebugField]
  public readonly RonElement[]? Values = values;
}
