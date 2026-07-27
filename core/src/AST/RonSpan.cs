namespace Ron;

public class RonSpan
{
  public Position start = null!;
  public Position end = null!;
  public int startOffset;
  public int endOffset;

  public static RonSpan From(Superpower.Model.Position start, Superpower.Model.Position end)
  {
    return new RonSpan
    {
      start = new(start.Line, start.Column),
      end = new(end.Line, end.Column),
      startOffset = start.Absolute,
      endOffset = end.Absolute,
    };
  }
}

public class Position(int line = 0, int col = 0)
{
  public int line = line;
  public int col = col;
}
