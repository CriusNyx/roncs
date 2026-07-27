namespace Ron;

public abstract class StructBody;

public class TupleBody : StructBody
{
  public RonSpan? openParen;
  public Trivia? leading;
  public IEnumerable<TupleElement>? elements;
  public Trivia? trailing;
  public RonSpan? closeParen;
}

public class FieldsBody : StructBody
{
  public RonSpan? openBrace;
  public Trivia? leading;

  public IEnumerable<StructField>? fields;
  public Trivia? trailing;
  public RonSpan? closeBrace;
}

public class StructField
{
  public Trivia? leading;
  public Ident? name;
  public Trivia? preColon;
  public RonSpan? colon;
  public Trivia? postColon;
  public Expr? value;
  public Trivia? trailing;

  // Null if no comma.
  public RonSpan? comma;
}

public class StructExpr : Expr
{
  public RonSpan? span;
  public Ident? name;
  public Trivia? preBody;

  // Null if absent
  public StructBody? body;
}
