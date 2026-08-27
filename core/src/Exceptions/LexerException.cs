using Superpower.Model;

namespace RonCS.Exceptions;

/// <summary>
/// Ron document failed to be lexed.
/// </summary>
/// <param name="errorResult"></param>
public class LexerException(Result<TokenList<RonTokenKind>> errorResult) : Exception
{
  public readonly Result<TokenList<RonTokenKind>> errorResult = errorResult;
}
