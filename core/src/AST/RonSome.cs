using CriusNyx.Util;

[DebugPrint]
[Serializable]
public class RonSome(RonElement? value = null) : RonElement
{
  [DebugField]
  public RonElement? value = value;
}
