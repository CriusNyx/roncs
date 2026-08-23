using Superpower.Model;

namespace RonCS.Exceptions;

public class LexerException(Result<TokenList<RonTokenKind>> errorResult) : Exception
{
  public readonly Result<TokenList<RonTokenKind>> errorResult = errorResult;
}
