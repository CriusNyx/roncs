using CriusNyx.Results;
using CriusNyx.Results.Extensions;
using RonCS.AST;
using RonCS.Exceptions;
using Superpower.Model;

namespace RonCS;

internal static class RonExtensions
{
  /// <summary>
  /// Convert a parse result to a Ron result.
  /// </summary>
  /// <param name="tokenizerResult"></param>
  /// <returns></returns>
  public static Result<RonDocument, Exception> FromParseResult(
    this TokenListParserResult<RonTokenKind, RonDocument> tokenizerResult
  )
  {
    if (tokenizerResult.HasValue)
    {
      return tokenizerResult.Value.AsOk();
    }
    else
    {
      return new RonParseException(tokenizerResult).AsErr<Exception>();
    }
  }
}
