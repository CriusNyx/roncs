using CriusNyx.Util;

namespace Ron;

public abstract class TriviaElement { }

[DebugPrint]
public class Whitespace : TriviaElement
{
  public RonSpan? span;

  [DebugField]
  public string? text;
}

public enum CommentKind
{
  Line,
  Block,
}

[DebugPrint]
public class Comment : TriviaElement
{
  public RonSpan? span;

  [DebugField]
  public string? text;

  [DebugField]
  public CommentKind? kind;
}

[DebugPrint]
public class Trivia
{
  public RonSpan? span;

  [DebugField]
  public IEnumerable<TriviaElement>? elements;
}
