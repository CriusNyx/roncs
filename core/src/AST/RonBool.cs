using CriusNyx.Util;

[Serializable]
[DebugPrint]
public class RonBool(bool value = false) : RonElement
{
  [DebugField]
  public bool Value = value;
}
