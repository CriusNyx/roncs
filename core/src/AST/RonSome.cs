namespace RonCS.AST;

/// <summary>
/// AST element for a ron some.
/// </summary>
/// <param name="value"></param>
[Serializable]
public class RonSome(RonElement? value = null) : RonElement
{
  /// <summary>
  /// The value inside the some.
  /// </summary>
  public RonElement? value = value;

  /// <inheritdoc/>
  public override string RonPrint(RonPrintOptions options)
  {
    return $"Some({value?.RonPrint(options)})";
  }
}
