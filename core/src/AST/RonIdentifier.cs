namespace RonCS.AST;

/// <summary>
/// A value that has an identifier.
/// </summary>
interface IIdentifier
{
  /// <summary>
  /// The string name of the identifier.
  /// </summary>
  /// <returns></returns>
  string IdentifierName();
}

/// <summary>
/// AST element for a ron identifier.
/// </summary>
/// <param name="name"></param>
[Serializable]
public class RonIdentifier(string? name = null) : RonElement, IIdentifier
{
  /// <summary>
  /// The name of the identifier.
  /// </summary>
  public string? Name = name;

  public override string RonPrint(RonPrintOptions options)
  {
    return Name ?? "";
  }

  string IIdentifier.IdentifierName()
  {
    return Name.NotNull("Value");
  }
}
