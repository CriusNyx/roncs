using CriusNyx.Util;

[DebugPrint]
[Serializable]
public class RonRawIdentifier(string? value = null) : RonElement
{
  [DebugField]
  public string? value = value;
}
