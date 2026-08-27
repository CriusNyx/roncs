using RonCS.AST;
using Superpower.Model;

namespace RonCS.Exceptions;

/// <summary>
/// Ron document failed to be parsed.
/// </summary>
/// <param name="result"></param>
public class RonParseException(TokenListParserResult<RonTokenKind, RonDocument> result) : Exception
{
  public readonly TokenListParserResult<RonTokenKind, RonDocument> Result = result;
}
