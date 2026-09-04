using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace RonCS.AST;

/// <summary>
/// Ron parser for numbers.
/// </summary>
public static class NumberParser
{
  #region Unsigned
  /// <summary>
  /// Unsigned binary number
  /// </summary>
  public static TextParser<RonUnsigned> UnsignedBinary_Parser = Span.EqualTo("0b")
    .Try()
    .IgnoreThen(
      StringParser
        .DigitBinary_Parser.AsDigitString()
        .Select(value => new RonUnsigned(UnsignedPrefix.binary, value))
    );

  /// <summary>
  /// Unsigned octal number
  /// </summary>
  public static TextParser<RonUnsigned> UnsignedOctal_Parser = Span.EqualTo("0o")
    .Try()
    .IgnoreThen(
      StringParser
        .DigitOctal_Parser.AsDigitString()
        .Select(value => new RonUnsigned(UnsignedPrefix.octal, value))
    );

  /// <summary>
  /// Unsigned hex number
  /// </summary>
  public static TextParser<RonUnsigned> UnsignedHex_Parser = Span.EqualTo("0x")
    .Try()
    .IgnoreThen(
      StringParser
        .DigitHexDecimal_Parser.AsDigitString()
        .Select(value => new RonUnsigned(UnsignedPrefix.hex, value))
    );

  /// <summary>
  /// Unsigned decimal number
  /// </summary>
  public static TextParser<RonUnsigned> UnsignedDecimal_Parser = StringParser
    .Digit_Parser.AsDigitString()
    .Select(value => new RonUnsigned(null, value));

  /// <summary>
  /// Unsigned number
  /// </summary>
  public static TextParser<RonUnsigned> Unsigned_Parser = Parse.OneOf(
    UnsignedBinary_Parser,
    UnsignedOctal_Parser,
    UnsignedHex_Parser,
    UnsignedDecimal_Parser
  );
  #endregion

  #region Integer
  /// <summary>
  /// Unsigned integer number.
  /// </summary>
  public static TextParser<IntegerSuffix> IntegerSuffix_Parser =
    ParseExtensions.EnumParser<IntegerSuffix>();

  /// <summary>
  /// Integer
  /// </summary>
  public static TextParser<RonInteger> Integer_Parser =
    from sign in Character.In('-', '+').OrNull()
    from unsigned in Unsigned_Parser
    from suffix in IntegerSuffix_Parser.OrNull()
    select new RonInteger(sign, unsigned, suffix);
  #endregion

  #region Byte

  /// <summary>
  /// Escaped byte
  /// </summary>
  public static TextParser<RonByte> EscapedByte_Parser =
    from escape in Character.EqualTo('\\')
    from content in Parse.OneOf(
      StringParser.EscapeByte_Parser,
      StringParser.EscapeAscii_Parser,
      StringParser.EscapeUnicode_Parser
    )
    select new RonByte(content.AsNotNull<INumberValue>("content"));

  /// <summary>
  /// Escaped ascii number
  /// </summary>
  public static TextParser<RonByte> ByteAscii_Parser = Character.AnyChar.Select(c => new RonByte(
    new RonAsciiLiteral(c)
  ));

  /// <summary>
  /// Byte
  /// </summary>
  public static TextParser<RonByte> ByteContent_Parser = Parse.OneOf(
    EscapedByte_Parser,
    ByteAscii_Parser
  );

  /// <summary>
  /// Byte
  /// </summary>
  public static TextParser<RonByte> Byte_Parser = Character
    .EqualTo('b')
    .IgnoreThen(ByteContent_Parser.Between(Character.EqualTo('\'')));
  #endregion

  #region Float
  /// <summary>
  /// Decimal digits portion of a floating point number, excluding the decimal.
  /// </summary>
  public static TextParser<string> FloatInt_Parser = StringParser
    .Digit_Parser.AsDigitString()
    .ThenIgnore(Parse.Not(Character.EqualTo('.')));

  /// <summary>
  /// Standard float.
  /// </summary>
  public static TextParser<string> FloatStd_Parser =
    from before in StringParser.Digit_Parser.AsDigitString()
    from dot in Span.EqualTo(".").ThenIgnore(Parse.Not(Character.EqualTo('.')))
    from after in StringParser.Digit_Parser.AsDigitString()!.OptionalOrDefault()
    select before + dot + after;

  /// <summary>
  /// ?
  /// </summary>
  public static TextParser<string> FloatFrac_Parser =
    from c in Span.EqualTo('.').ThenIgnore(Parse.Not(Character.EqualTo('.')))
    from digits in StringParser.Digit_Parser.AsDigitString()
    select c + digits;

  /// <summary>
  /// Exponent
  /// </summary>
  public static TextParser<RonExponent> FloatExp_Parser =
    from e in Character.In('e', 'E')
    from sign in Character.In('-', '+').OrNull()
    from leading in Character.EqualTo('_').AsString()
    from digits in StringParser.Digit_Parser.AsDigitString()
    select new RonExponent(e, sign, leading + digits);

  /// <summary>
  /// Standard float num
  /// </summary>
  public static TextParser<RonFloatNumber> StandardFloatNum_Parser =
    from digits in Parse.OneOf(FloatInt_Parser.Try(), FloatStd_Parser, FloatFrac_Parser)
    from exponent in FloatExp_Parser!.OptionalOrDefault()
    select new RonStandardFloat(digits, exponent).AsNotNull<RonFloatNumber>();

  /// <summary>
  /// Special float num
  /// </summary>
  public static TextParser<RonFloatNumber> SpecialFloatNum_Parser = ParseExtensions
    .EnumParser<SpecialFloatType>()
    .Select(value => new RonSpecialFloat(value).AsNotNull<RonFloatNumber>());

  /// <summary>
  /// Float number part
  /// </summary>
  public static TextParser<RonFloatNumber> FloatNum_Parser = Parse.OneOf(
    StandardFloatNum_Parser,
    SpecialFloatNum_Parser
  );

  /// <summary>
  /// Float
  /// </summary>
  public static TextParser<RonFloat> Float_Parser =
    from sign in Character.In('+', '-').OrNull()
    from num in FloatNum_Parser
    from suffix in ParseExtensions.EnumParser<FloatSuffix>().OrNull()
    select new RonFloat(sign, num, suffix);

  /// <summary>
  /// Number
  /// </summary>
  public static TextParser<INumberValue> Number_Parser = Parse.OneOf(
    Byte_Parser.Try().AsNumberValue(),
    Integer_Parser
      .AsNumberValue()
      .ThenIgnore(
        Parse.Not(
          Parse.OneOf(
            ParseExtensions.EnumParser<FloatSuffix>().Ignore(),
            Character.In('.', 'e', 'E').Ignore()
          )
        )
      )
      .Try(),
    Float_Parser.AsNumberValue()
  );
  #endregion

  #region Extensions
  internal static TextParser<char> OrUnderscore(this TextParser<char> source)
  {
    return Parse.OneOf(source, Character.EqualTo('_'));
  }

  internal static TextParser<string> Concat(
    this TextParser<char> original,
    TextParser<char> next = null!
  )
  {
    if (next is not null)
    {
      return original.Then(first => next.Many().Select((rest) => new string([first, .. rest])));
    }
    else
    {
      return original.Many().AsString();
    }
  }

  internal static TextParser<string> AsDigitString(this TextParser<char> digits)
  {
    return digits.Concat(digits.OrUnderscore());
  }

  internal static TextParser<string> AsString(this TextParser<IEnumerable<char>> source)
  {
    return source.Select(chars => new string(chars.ToArray()));
  }

  internal static TextParser<string> AsString(this TextParser<char[]> charParser)
  {
    return charParser.Select(chars => new string(chars));
  }

  internal static TextParser<string> AsString(this TextParser<TextSpan> source)
  {
    return source.Select(x => x.ToStringValue());
  }

  internal static TextParser<RonUnsigned> AsUnsignedValue<T>(this TextParser<T> source)
    where T : RonUnsigned
  {
    return source.Select(x => x.AsNotNull<RonUnsigned>());
  }

  internal static TextParser<INumberValue> AsNumberValue<T>(this TextParser<T> source)
    where T : INumberValue
  {
    return source.Select(x => x.AsNotNull<INumberValue>());
  }

  internal static TextParser<Struct?> OrNull<Struct>(this TextParser<Struct> source)
    where Struct : struct
  {
    return source.Select(x => (Struct?)x).OptionalOrDefault();
  }

  internal static TextParser<object> Ignore<T>(this TextParser<T> source)
  {
    return source.Value(null as object)!;
  }

  /// <summary>
  /// Do not use.
  /// </summary>
  /// <param name="prefix"></param>
  /// <returns></returns>
  public static string ToPrefixString(this UnsignedPrefix? prefix)
  {
    switch (prefix)
    {
      case UnsignedPrefix.binary:
        return "0b";
      case UnsignedPrefix.octal:
        return "0o";
      case UnsignedPrefix.hex:
        return "0x";
      default:
        return "";
    }
  }
  #endregion
}
