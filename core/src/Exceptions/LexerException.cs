using Superpower.Model;

namespace RonCS;

public class LexerException(Result<TokenList<RonTokenKind>> errorResult) : Exception
{
  public readonly Result<TokenList<RonTokenKind>> errorResult = errorResult;
}
