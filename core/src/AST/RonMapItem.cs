using CriusNyx.Util;

[DebugPrint]
[Serializable]
public class RonMapItem(RonElement? key, RonElement? value) : RonElement
{
  [DebugField]
  public RonElement? Key = key;

  [DebugField]
  public RonElement? Value = value;
}
