namespace RonCS.AST;

/// <summary>
/// Parent class for a ron element.
/// </summary>
[Serializable]
public abstract class RonElement
{
  /// <summary>
  /// Print the ron element as a Ron string.
  /// </summary>
  /// <param name="options"></param>
  /// <returns></returns>
  public abstract string RonPrint(RonPrintOptions options);
}

/// <summary>
/// Ron element extensions.
/// </summary>
public static class RonElementExtensions
{
  /// <summary>
  /// If the element is an identifier return it's name. Otherwise returns null.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static string? IdentifierName(this RonElement? element)
  {
    if (element is IIdentifier ident)
    {
      return ident.IdentifierName();
    }
    return null;
  }
}
