namespace Ron;

public enum StringKind
{
  Regular,
  Raw,
}

public class StringExpr : Expr
{
  public RonSpan? span;

  // public string? raw;
  public string? value;
  public StringKind? stringKind;
}
