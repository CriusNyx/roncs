[Serializable]
public class RonElement { }

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
