namespace RonCS.AST;

/// <summary>
/// AST element representing a ron none.
/// </summary>
[Serializable]
public class RonNone : RonElement
{
  /// <inheritdoc/>
  public override string RonPrint(RonPrintOptions options)
  {
    return "None";
  }
}
