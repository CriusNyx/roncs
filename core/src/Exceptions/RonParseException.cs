using RonCS;
using Superpower.Model;

public class RonParseException(TokenListParserResult<RonTokenKind, RonDocument> result) : Exception
{
  public readonly TokenListParserResult<RonTokenKind, RonDocument> Result = result;
}
