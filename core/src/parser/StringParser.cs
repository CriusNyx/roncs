using System.Diagnostics;
using CriusNyx.Util;
using Superpower;
using Superpower.Parsers;

public abstract class StringContent
{
  public abstract string Evaluate();
}

[DebugPrint]
public class StringValue(StringContent[] content) : RonElement
{
  [DebugField]
  public StringContent[] content = content;

  public string Evaluate() => content.Select(x => x.Evaluate()).StringJoin();
};

[DebugPrint]
public class StringLit(string value) : StringContent
{
  [DebugField]
  public string value = value;

  public override string Evaluate()
  {
    return value;
  }
}

[DebugPrint]
public class AsciiEscape(char source) : StringContent
{
  [DebugField]
  public char? source = source;

  public override string Evaluate()
  {
    return source switch
    {
      '\'' => "'",
      '"' => "\"",
      '\\' => "\\",
      'n' => "\n",
      'r' => "\r",
      't' => "\t",
      '0' => "\0",
      _ => throw new NotImplementedException(),
    };
  }
}

[DebugPrint]
public class ByteEscape(char left, char right) : StringContent
{
  [DebugField]
  public char? left = left;

  [DebugField]
  public char? right = right;

  public override string Evaluate()
  {
    char l = (char)left.NotNull("left")!;
    char r = (char)right.NotNull("left")!;
    var b = byte.Parse([l, r], System.Globalization.NumberStyles.HexNumber);
    return ((char)b).ToString();
  }
}

[DebugPrint]
public class UnicodeEscape(string source) : StringContent
{
  [DebugField]
  public string source = source;

  public override string Evaluate()
  {
    uint u = uint.Parse(source, System.Globalization.NumberStyles.HexNumber);
    if (u >= 0x10000 && u <= 0x10FFFF)
    {
      uint uPrime = u - 0x10000;
      uint high = (uPrime >> 10) + 0xD800;
      uint low = (uPrime & 0x3ff) + 0xDC00;
      return new string([(char)high, (char)low]);
    }
    else if (u <= 0xD7FF || (u >= 0xE000 && u <= 0xFFFF))
    {
      return new string([(char)u]);
    }
    throw new InvalidOperationException();
  }
}

[DebugPrint]
public class StringRawContent(StringContent content) : StringContent
{
  [DebugField]
  public StringContent content = content;

  public override string Evaluate()
  {
    return content.Evaluate();
  }
}

[DebugPrint]
public class StringRawLit(string source) : StringContent
{
  [DebugField]
  public string source = source;

  public override string Evaluate()
  {
    return source;
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
