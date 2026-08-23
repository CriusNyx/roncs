using RonCS;
using RonCS.AST;
using Superpower.Model;

namespace RonCS.Exceptions;

public class RonParseException(TokenListParserResult<RonTokenKind, RonDocument> result) : Exception
{
  public readonly TokenListParserResult<RonTokenKind, RonDocument> Result = result;
}
