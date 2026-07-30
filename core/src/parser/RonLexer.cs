using CriusNyx.Util;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace RonCS;

public enum RonTokenKind
{
  // Symbols
  Comma,
  Colon,
  Hash,

  // Operators
  RangeExclusive,
  RangeInclusive,

  // Braces and Parens
  OpenParen,
  ClosedParen,
  OpenSquare,
  ClosedSquare,
  OpenCurly,
  ClosedCurly,

  // Options
  Some,
  None,

  // Primitives
  Char,
  True,
  False,

  // Numbers
  Number,

  // Strings
  String,

  // Identifiers
  Identifier,
  RawIdentifier,
};

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

  public static Tokenizer<RonTokenKind> Tokenizer { get; private set; } =
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
      .Match(
        Character
          .EqualTo('\'')
          .IgnoreThen(
            Parse.OneOf(
              Span.EqualTo(@"\'").Value(true).Try(),
              Span.EqualTo(@"\\").Value(true),
              Character.Except('\'').Value(true)
            )
          )
          .IgnoreThen(Character.EqualTo('\'')),
        RonTokenKind.Char
      )
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

  public static TokenList<RonTokenKind> Tokenize(string source)
  {
    return Tokenizer.Tokenize(source);
  }
}
