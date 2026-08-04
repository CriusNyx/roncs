using CriusNyx.Util;

[DebugPrint]
[Serializable]
public class RonRawIdentifier(string? value = null) : RonElement, IIdentifier
{
  [DebugField]
  public string? value = value;

  public string Value()
  {
    return value.NotNull("Identifier");
  }

  public override string RonPrint(RonPrintOptions options)
  {
    return $"r#{value}";
  }
}
