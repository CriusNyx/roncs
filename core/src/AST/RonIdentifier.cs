using CriusNyx.Util;

[DebugPrint]
[Serializable]
public class RonIdentifier(string? value = null) : RonElement
{
  [DebugField]
  public string? Value = value;
}
