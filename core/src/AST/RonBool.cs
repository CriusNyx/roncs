namespace RonCS.AST;

/// <summary>
/// AST element for a RonBool
/// </summary>
/// <param name="value"></param>
[Serializable]
public class RonBool(bool value = false) : RonElement
{
  /// <summary>
  /// The value of the boolean.
  /// </summary>
  public bool Value = value;

  /// <inheritdoc/>
  public override string RonPrint(RonPrintOptions options)
  {
    return Value.ToString().ToLower();
  }
}
