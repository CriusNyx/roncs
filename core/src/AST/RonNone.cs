using CriusNyx.Util;

[DebugPrint]
[Serializable]
public class RonNone : RonElement
{
  public override string RonPrint(RonPrintOptions options)
  {
    return "None";
  }
}
