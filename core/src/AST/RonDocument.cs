using CriusNyx.Util;

[DebugPrint]
[Serializable]
public class RonDocument(RonElement? value) : RonElement
{
  [DebugField]
  public RonElement? Value = value;
}
