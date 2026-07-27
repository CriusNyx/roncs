namespace Ron;

public class SeqItem
{
  public Trivia? leading;
  public Expr? expr;
  public Trivia? trailing;

  // Null if no comma
  public RonSpan? comma;
}

public class SeqExpr : Expr
{
  public RonSpan? span;
  public RonSpan? openBracket;
  public Trivia? leading;
  public IEnumerable<SeqItem>? items;
  public Trivia? trailing;
  public RonSpan? closeBracket;
}
