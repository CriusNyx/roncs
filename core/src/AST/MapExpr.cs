namespace Ron;

public class MapEntry
{
  public Trivia? leading;
  public Expr? key;
  public Trivia? preColon;
  public RonSpan? colon;
  public Trivia? postColon;
  public Expr? value;
  public Trivia? trailing;

  // Null if no comma
  public RonSpan? comma;
}

public class MapExpr : Expr
{
  public RonSpan? span;
  public RonSpan? openBrace;
  public Trivia? leading;
  public IEnumerable<MapEntry>? entries;
  public Trivia? trailing;
  public RonSpan? closeBrace;
}
