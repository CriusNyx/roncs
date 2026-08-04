namespace RonCS;

public partial class SerializationContext
{
  public RonElement ToAST(object source)
  {
    return source switch
    {
      string str => new StringValue([new StringLit(str)]),
      _ => throw new NotFiniteNumberException(),
    };
  }
}
