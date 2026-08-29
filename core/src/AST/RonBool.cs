namespace RonCS.AST;

/// <summary>
/// AST element for a RonBool
/// </summary>
/// <param name="value"></param>
[Serializable]
public class RonBool(bool value = false) : RonElement
{
  public bool Value = value;

  public override string RonPrint(RonPrintOptions options)
  {
    return Value.ToString().ToLower();
  }
}
