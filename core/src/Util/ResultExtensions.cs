using System.Formats.Asn1;
using CriusNyx.Results;
using Superpower.Model;

public static class ResultExtensions
{
  public static Result<TokenKind, Exception> IntoResult<TokenKind>(
    this Result<TokenKind> source,
    Func<Result<TokenKind>, Exception> exceptionBuilder
  )
  {
    if (source.HasValue)
    {
      return source.Value;
    }
    return exceptionBuilder(source);
  }

  public static Result<AST, Exception> IntoResult<TokenKind, AST>(
    this TokenListParserResult<TokenKind, AST> source,
    Func<TokenListParserResult<TokenKind, AST>, Exception> exceptionBuilder
  )
  {
    if (source.HasValue)
    {
      return source.Value;
    }
    return exceptionBuilder(source);
  }
}
