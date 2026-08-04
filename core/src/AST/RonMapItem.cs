using CriusNyx.Util;

[DebugPrint]
[Serializable]
public class RonMapItem(RonElement? key, RonElement? value) : RonElement
{
  [DebugField]
  public RonElement? Key = key;

  [DebugField]
  public RonElement? Value = value;

  public override string RonPrint(RonPrintOptions options)
  {
    switch (options.mode)
    {
      case RonPrintMode.Compact:
        return Key?.RonPrint(options) + ":" + Value?.RonPrint(options);
      case RonPrintMode.Pretty:
        return Key?.RonPrint(options) + ": " + Value?.RonPrint(options);
      default:
        throw options.mode.AsEnumException();
    }
  }
}
