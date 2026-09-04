using RonCS.Exceptions;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace RonCS.AST;

/// <summary>
/// Tokens for the RON language.
/// </summary>
public enum RonTokenKind
{
  // Symbols
  /// <summary>
  /// ,
  /// </summary>
  Comma,

  /// <summary>
  /// ;
  /// </summary>
  Colon,

  /// <summary>
  /// #
  /// </summary>
  Hash,

  // Operators
  /// <summary>
  /// ..
  /// </summary>
  RangeExclusive,

  /// <summary>
  /// ..=
  /// </summary>
  RangeInclusive,

  // Braces and Parens
  /// <summary>
  /// (
  /// </summary>
  OpenParen,

  /// <summary>
  /// )
  /// </summary>
  ClosedParen,

  /// <summary>
  /// [
  /// </summary>
  OpenSquare,

  /// <summary>
  /// ]
  /// </summary>
  ClosedSquare,

  /// <summary>
  /// {
  /// </summary>
  OpenCurly,

  /// <summary>
  /// }
  /// </summary>
  ClosedCurly,

  // Options
  /// <summary>
  /// Some
  /// </summary>
  Some,

  /// <summary>
  /// None
  /// </summary>
  None,

  // Primitives
  /// <summary>
  /// Any XID character.
  /// </summary>
  Char,

  /// <summary>
  /// true
  /// </summary>
  True,

  /// <summary>
  /// false
  /// </summary>
  False,

  // Numbers
  /// <summary>
  /// Any ron number
  /// </summary>
  Number,

  // Strings
  /// <summary>
  /// Any ron string
  /// </summary>
  String,

  // Identifiers
  /// <summary>
  /// XID identifier
  /// </summary>
  Identifier,

  /// <summary>
  /// Raw identifier
  /// </summary>
  RawIdentifier,
};

/// <summary>
/// Base class for ron tokenizer
/// </summary>
public class RonLexer
{
  static bool IsIdentStdFirst(char c)
  {
    return XID.IsXidStart(c) || c == '_';
  }

  static bool IsIdentStdRest(char c)
  {
    return XID.IsXidContinue(c);
  }

  static bool IsIdentRawRest(char c)
  {
    return IsIdentStdRest(c) || c == '.' || c == '+' || c == '-';
  }

  static TextParser<bool> BlockCommentParser = Parse.Ref(() =>
    Span.EqualTo("/*")
      .IgnoreThen(Span.Regex("[^\\*\\/|^\\/\\*]*").Optional())
      .IgnoreThen(BlockCommentParser.NotNull("NestedBlockCommentParser").Optional())
      .IgnoreThen(Span.Regex("[^\\*\\/|^\\/\\*]*").Optional())
      .IgnoreThen(Span.EqualTo("*/"))
      .Value(true)
  );

  internal static Tokenizer<RonTokenKind> Tokenizer { get; private set; } =
    new TokenizerBuilder<RonTokenKind>()
      // Whitespace
      .Ignore(Span.WhiteSpace)
      // Comments
      .Ignore(
        Span.EqualTo("//")
          .IgnoreThen(Character.Except('\n').Many())
          .ThenIgnore(Character.EqualTo('\n').Optional())
      )
      .Ignore(BlockCommentParser)
      // Operators
      .Match(Span.EqualTo("..=").Try(), RonTokenKind.RangeInclusive)
      .Match(Span.EqualTo(".."), RonTokenKind.RangeExclusive)
      // Numbers
      .Match(NumberParser.Number_Parser.Try(), RonTokenKind.Number)
      .Match(StringParser.String_Parser.Try(), RonTokenKind.String)
      // Symbols
      .Match(Character.EqualTo(':'), RonTokenKind.Colon)
      .Match(Character.EqualTo(','), RonTokenKind.Comma)
      // Braces and Parens
      .Match(Character.EqualTo('('), RonTokenKind.OpenParen)
      .Match(Character.EqualTo(')'), RonTokenKind.ClosedParen)
      .Match(Character.EqualTo('['), RonTokenKind.OpenSquare)
      .Match(Character.EqualTo(']'), RonTokenKind.ClosedSquare)
      .Match(Character.EqualTo('{'), RonTokenKind.OpenCurly)
      .Match(Character.EqualTo('}'), RonTokenKind.ClosedCurly)
      // Primitives
      .Match(Span.EqualTo("true"), RonTokenKind.True)
      .Match(Span.EqualTo("false"), RonTokenKind.False)
      // Numbers
      .Match(Numerics.Integer, RonTokenKind.Number)
      // Options
      .Match(Span.EqualTo("Some"), RonTokenKind.Some)
      .Match(Span.EqualTo("None"), RonTokenKind.None)
      // Character
      .Match(StringParser.Char_Parser, RonTokenKind.Char)
      // Identifier
      .Match(
        Span.EqualTo("r#")
          .IgnoreThen(Character.Matching(IsIdentRawRest, "ident_raw_rest").Many())
          .Try(),
        RonTokenKind.RawIdentifier
      )
      .Match(
        Character
          .Matching(IsIdentStdFirst, "ident_std_start")
          .IgnoreThen(Character.Matching(IsIdentStdRest, "ident_std_continue").Many())
          .Value(true),
        RonTokenKind.Identifier
      )
      .Build();

  /// <summary>
  /// Try to tokenize the input.
  /// </summary>
  /// <param name="source">The source code</param>
  /// <param name="tokens">The token list if successful</param>
  /// <param name="exception">The exception if failed</param>
  /// <returns></returns>
  public static bool TryTokenize(
    string source,
    out TokenList<RonTokenKind> tokens,
    out Exception exception
  )
  {
    tokens = default;
    exception = null!;

    var result = Tokenizer.TryTokenize(source);
    if (result.HasValue)
    {
      tokens = result.Value;
      return true;
    }
    else
    {
      exception = new LexerException(result);
      return false;
    }
  }

  /// <summary>
  /// Tokenize the ron document and return the token list
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static TokenList<RonTokenKind> Tokenize(string source)
  {
    if (TryTokenize(source, out var list, out var exception))
    {
      return list;
    }
    else
    {
      throw exception;
    }
  }
}
