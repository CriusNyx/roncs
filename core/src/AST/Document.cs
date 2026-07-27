namespace Ron;

public class Document
{
  // Maybe don't need this.
  public string? source;
  public Trivia? leading;
  public IEnumerable<RonAttribute>? attributes;
  public Trivia? preValue;
  public Expr? value;
  public Trivia? trailing;
}
