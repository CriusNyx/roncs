namespace RonCS.AST;

[Serializable]
public abstract class RonElement
{
  public abstract string RonPrint(RonPrintOptions options);
}

public static class RonElementExtensions
{
  public static string? IdentifierName(this RonElement? element)
  {
    if (element is IIdentifier ident)
    {
      return ident.Value();
    }
    return null;
  }
}
