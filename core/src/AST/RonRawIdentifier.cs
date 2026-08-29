namespace RonCS.AST;

/// <summary>
/// AST element for a raw identifier.
/// </summary>
/// <param name="name"></param>
[Serializable]
public class RonRawIdentifier(string? name = null) : RonElement, IIdentifier
{
  /// <summary>
  /// The name of the identifier.
  /// </summary>
  public string? Name = name;

  public string IdentifierName()
  {
    return Name.NotNull("Identifier");
  }

  public override string RonPrint(RonPrintOptions options)
  {
    return $"r#{Name}";
  }
}
