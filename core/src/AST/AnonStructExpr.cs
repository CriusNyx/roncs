namespace Ron;

public class AnonStructExpr : Expr
{
  public RonSpan? span;
  public RonSpan? openParen;
  public Trivia? leading;

  public IEnumerable<StructField>? fields;
  public Trivia? trailing;
  public RonSpan? closeParen;
}
