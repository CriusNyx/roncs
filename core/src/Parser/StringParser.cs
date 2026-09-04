using Superpower;
using Superpower.Parsers;

namespace RonCS.AST;

/// <summary>
/// Parser for Ron strings.
/// </summary>
public static class StringParser
{
  # region Digits
  /// <summary>
  /// Digit
  /// </summary>
  public static TextParser<char> Digit_Parser = Character.Digit;

  /// <summary>
  /// DigitBinary
  /// </summary>
  public static TextParser<char> DigitBinary_Parser = Character.In('0', '1');

  /// <summary>
  /// DigitOctal
  /// </summary>
  public static TextParser<char> DigitOctal_Parser = Character.In(
    '0',
    '1',
    '2',
    '3',
    '4',
    '5',
    '6',
    '7'
  );

  /// <summary>
  /// DigitHexDecimal
  /// </summary>
  public static TextParser<char> DigitHexDecimal_Parser = Character.HexDigit;
  #endregion

  /// <summary>
  /// Character or escaped character
  /// </summary>
  public static TextParser<RonElement> Char_Parser = Parse
    .OneOf(
      Span.EqualTo("\\'").Value(new RonChar('\'') as RonElement).Try(),
      Span.EqualTo("\\\\").Value(new RonChar('\\') as RonElement).Try(),
      Character.Except('\\').Select(c => new RonChar(c) as RonElement)
    )
    .Between(Character.EqualTo('\''));

  /// <summary>
  /// Ascii character
  /// </summary>
  public static TextParser<StringContent> EscapeAscii_Parser = Character
    .In('\'', '"', '\\', 'n', 'r', 't', '0')
    .Select(x => new RonAsciiEscape(x) as StringContent);

  /// <summary>
  /// Byte
  /// </summary>
  public static TextParser<StringContent> EscapeByte_Parser =
    from x in Character.EqualTo('x')
    from left in DigitHexDecimal_Parser
    from right in DigitHexDecimal_Parser
    select new RonByteEscape(left, right) as StringContent;

  /// <summary>
  /// Escaped Unicode
  /// </summary>
  public static TextParser<StringContent> EscapeUnicode_Parser =
    from x in Character.EqualTo('u')
    from digits in DigitHexDecimal_Parser
      .UpTo(6)
      .AsString()
      .Between(Character.EqualTo('{'), Character.EqualTo('}'))
    select new RonUnicodeEscape(digits) as StringContent;

  /// <summary>
  /// Any escaped string element
  /// </summary>
  public static TextParser<StringContent> StringEscapeContent_Parser = Parse.OneOf(
    EscapeAscii_Parser,
    EscapeByte_Parser,
    EscapeUnicode_Parser
  );

  /// <summary>
  /// Escaped string
  /// </summary>
  public static TextParser<StringContent> StringEscape_Parser = Character
    .EqualTo('\\')
    .IgnoreThen(StringEscapeContent_Parser)
    .Named("StringEscape");

  /// <summary>
  /// Standard string
  /// </summary>
  public static TextParser<RonString> StringSTD_Parser = Parse
    .OneOf(
      StringEscape_Parser.Try(),
      Character
        .Except('"')
        .AsString(true)
        .Select(characters => new RonStringLit(characters) as StringContent)
    )
    .Many()
    .Between(Character.EqualTo('"'))
    .Select(content => new RonString(content))
    .Named("StringSTD");

  /// <summary>
  /// Raw content parser
  /// </summary>
  public static TextParser<StringContent> StringRawContent_Parser = Parse.Ref(() =>
    Parse.OneOf(
      StringRawContent_Parser
        .NotNull("StringRawContent_Parser")
        .Between(Character.EqualTo('#'))
        .Select(x => new RonStringRawContent(x) as StringContent),
      Character
        .Except('"')
        .AsString()
        .Between(Character.EqualTo('"'))
        .Select(value => new RonStringRawLit(value) as StringContent)
    )
  );

  /// <summary>
  /// Raw string parser
  /// </summary>
  public static TextParser<RonString> StringRaw_Parser = Character
    .EqualTo('r')
    .IgnoreThen(StringRawContent_Parser.Select(x => new RonString([x])));

  /// <summary>
  /// String parser
  /// </summary>
  public static TextParser<RonString> String_Parser = Parse.OneOf(
    StringSTD_Parser,
    StringRaw_Parser
  );
}
