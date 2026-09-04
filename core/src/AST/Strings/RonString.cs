using Superpower;

namespace RonCS.AST;

/// <summary>
/// AST element representing a string.
/// </summary>
/// <param name="content"></param>
public class RonString(params StringContent[] content) : RonElement
{
  /// <summary>
  /// The content of the RON string.
  /// </summary>
  public StringContent[] content = content;

  /// <summary>
  /// Evaluate this AST element as a C# string.
  /// </summary>
  /// <returns></returns>
  public string Evaluate() => content.Select(x => x.EvaluateString()).StringJoin();

  /// <inheritdoc/>
  public override string RonPrint(RonPrintOptions options)
  {
    return '"' + content.Select(x => x.Serialize()).StringJoin() + '"';
  }
};
