namespace RonCS.AST;

/// <summary>
/// AST element for a RonChar.
/// </summary>
/// <param name="value"></param>
public class RonChar(char value) : RonElement
{
  /// <summary>
  /// The value of the character.
  /// </summary>
  public char Value = value;

  /// <inheritdoc/>
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
