using CriusNyx.Util;

[Serializable]
[DebugPrint]
public class RonBool(bool value = false) : RonElement
{
  [DebugField]
  public bool Value = value;

  public override string RonPrint(RonPrintOptions options)
  {
    return Value.ToString().ToLower();
  }
}
