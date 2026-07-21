using CriusNyx.Util;
using Superpower;
using Superpower.Parsers;

public abstract class StringContent
{
  public abstract string Evaluate();
}

public class StringValue(StringContent[] content) : RonElement
{
  public StringContent[] content = content;
};

public class StringLit(string value) : StringContent
{
  public string value = value;

  public override string Evaluate()
  {
    return value;
  }
}

public class AsciiEscape(char source) : StringContent
{
  public char? source = source;

  public override string Evaluate()
  {
    throw new NotImplementedException();
  }
}

public class ByteEscape(char left, char right) : StringContent
{
  public char? left = left;
  public char? right = right;

  public override string Evaluate()
  {
    throw new NotImplementedException();
  }
}

public class UnicodeEscape(string source) : StringContent
{
  public string source = source;

  public override string Evaluate()
  {
    throw new NotImplementedException();
  }
}

public class StringRawContent(StringContent content) : StringContent
{
  public StringContent content = content;

  public override string Evaluate()
  {
    throw new NotImplementedException();
  }
}

public class StringRawLit(string source) : StringContent
{
  public string source = source;

  public override string Evaluate()
  {
    throw new NotImplementedException();
  }
}

public static class StringParser
{
  # region Digits
  public static TextParser<char> Digit_Parser = Character.Digit;
  public static TextParser<char> DigitBinary_Parser = Character.In('0', '1');
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
  public static TextParser<char> DigitHexDecimal_Parser = Character.HexDigit;
  #endregion

  public static TextParser<StringContent> EscapeAscii_Parser = Character
    .In('\'', '"', '\\', 'n', 'r', 't', '0')
    .Select(x => new AsciiEscape(x) as StringContent);

  public static TextParser<StringContent> EscapeByte_Parser =
    from x in Character.EqualTo('x')
    from left in DigitHexDecimal_Parser
    from right in DigitHexDecimal_Parser
    select new ByteEscape(left, right) as StringContent;

  public static TextParser<StringContent> EscapeUnicode_Parser =
    from x in Character.EqualTo('u')
    from digits in DigitHexDecimal_Parser
      .UpTo(6)
      .AsString()
      .Between(Character.EqualTo('{'), Character.EqualTo('}'))
    select new UnicodeEscape(digits) as StringContent;

  public static TextParser<StringContent> StringEscapeContent_Parser = Parse.OneOf(
    EscapeAscii_Parser,
    EscapeByte_Parser,
    EscapeUnicode_Parser
  );

  public static TextParser<StringContent> StringEscape_Parser = Character
    .EqualTo('\\')
    .IgnoreThen(StringEscapeContent_Parser)
    .Named("StringEscape");

  public static TextParser<StringValue> StringSTD_Parser = Parse
    .OneOf(
      StringEscape_Parser.Try(),
      Character
        .Except('"')
        .AsString(true)
        .Select(characters => new StringLit(characters) as StringContent)
    )
    .Many()
    .Between(Character.EqualTo('"'))
    .Select(content => new StringValue(content))
    .Named("StringSTD");

  public static TextParser<StringContent> StringRawContent_Parser = Parse.Ref(() =>
    Parse.OneOf(
      StringRawContent_Parser
        .NotNull("StringRawContent_Parser")
        .Between(Character.EqualTo('#'))
        .Select(x => new StringRawContent(x) as StringContent),
      Character
        .Except('"')
        .AsString()
        .Between(Character.EqualTo('"'))
        .Select(value => new StringRawLit(value) as StringContent)
    )
  );

  public static TextParser<StringValue> StringRaw_Parser = Character
    .EqualTo('r')
    .IgnoreThen(StringRawContent_Parser.Select(x => new StringValue([x])));

  public static TextParser<StringValue> String_Parser = Parse.OneOf(
    StringSTD_Parser,
    StringRaw_Parser
  );
}
