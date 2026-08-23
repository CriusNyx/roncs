namespace RonCS.AST;

public class RonChar(char value) : RonElement
{
  public char Value = value;

  public override string RonPrint(RonPrintOptions options)
  {
    string c = Value switch
    {
      '\\' => "\\\\",
      '\'' => "\\\'",
      _ => Value.ToString(),
    };
    return $"'{c}'";
  }
}
