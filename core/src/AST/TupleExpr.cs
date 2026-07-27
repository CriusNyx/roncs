namespace Ron;

public class TupleElement
{
  public Trivia? leading;
  public Expr? expr;
  public RonSpan? trailing;

  // Null if no comma
  public RonSpan? comma;
}

public class TupleExpr : Expr
{
  public RonSpan? span;
  public RonSpan? openParen;
  public Trivia? leading;

  public IEnumerable<TupleElement>? elements;
  public Trivia? trailing;
  public RonSpan? closeParen;
}
