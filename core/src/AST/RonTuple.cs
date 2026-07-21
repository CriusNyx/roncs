using CriusNyx.Util;

[DebugPrint]
[Serializable]
public class RonTuple(RonElement[]? values = null) : RonElement
{
  [DebugField]
  public RonElement[]? Values = values;
}
