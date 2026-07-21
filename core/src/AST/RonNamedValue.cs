using CriusNyx.Util;

[DebugPrint]
[Serializable]
public class RonNamedValue(RonElement? name = null, RonElement? value = null) : RonElement
{
  [DebugField]
  public RonElement? name = name;

  [DebugField]
  public RonElement? value = value;
}
