using CriusNyx.Util;

[DebugPrint]
[Serializable]
public class RonMap(RonElement[]? values = null) : RonElement
{
  [DebugField]
  public RonElement[]? Values = values;
}
