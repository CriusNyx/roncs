using CriusNyx.Util;

interface IIdentifier
{
  string Value();
}

[DebugPrint]
[Serializable]
public class RonIdentifier(string? value = null) : RonElement, IIdentifier
{
  [DebugField]
  public string? Value = value;

  public override string RonPrint(RonPrintOptions options)
  {
    return Value ?? "";
  }

  string IIdentifier.Value()
  {
    return Value.NotNull("Value");
  }
}
