namespace Ron;

public enum NumberKind
{
  Integer,
  NegativeInteger,
  Float,
  SpecialFloat,
}

public class NumberExpr : Expr
{
  public RonSpan? span;
  public string? raw;
  public NumberKind? kind;
}
