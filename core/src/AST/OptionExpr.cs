namespace Ron;

public class OptionValue
{
  public RonSpan? openParen;
  public Trivia? leading;
  public Expr? expr;
  public Trivia? trailing;
  public RonSpan? closeParen;
}

public class OptionExpr : Expr
{
  public RonSpan? span;

  // Null if none
  public OptionValue? value;
}
