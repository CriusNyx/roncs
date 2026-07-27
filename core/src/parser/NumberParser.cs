using System.Collections;
using Ron;
using Superpower;
using Superpower.Parsers;

public abstract class NumberValue
{
  public abstract string GenerateSource();
}

#region Unsigned Value
public class UnsignedValue(UnsignedPrefix? prefix = null, string? digits = null) : NumberValue
{
  public UnsignedPrefix? prefix = prefix;
  public string? digits = digits;

  public override string GenerateSource()
  {
    return digits
      + prefix switch
      {
        UnsignedPrefix.binary => "0b",
        UnsignedPrefix.hex => "0x",
        UnsignedPrefix.octal => "0o",
        _ => "",
      };
  }
}

public enum UnsignedPrefix
{
  binary = 2,
  octal = 8,
  hex = 16,
}

#endregion

#region Integer Value
public enum IntegerSuffix
{
  i8,
  i16,
  i32,
  i64,
  i128,
  u8,
  u16,
  u32,
  u64,
  u128,
}

public class IntegerValue(
  char? sign = null,
  UnsignedValue? digits = null,
  IntegerSuffix? integerSuffix = null
) : NumberValue
{
  public char? sign = sign;
  public UnsignedValue? digits = digits;
  public IntegerSuffix? integerSuffix = integerSuffix;

  public override string GenerateSource()
  {
    return sign + digits?.GenerateSource() + integerSuffix;
  }
}
#endregion

#region Byte Value
public class ByteValue(string source) : NumberValue
{
  public string source = source;

  public override string GenerateSource()
  {
    return source;
  }
}
#endregion

#region Float Value
public class FloatExponent(char? e, char? sign, string? digits)
{
  public char? e = e;
  public char? sign = sign;
  public string? digits = digits;

  public string GenerateSource()
  {
    return e + sign + digits;
  }
}

public abstract class FloatNum : NumberValue { };

public class StandardFloatNum(string? digits, FloatExponent? exponent) : FloatNum
{
  public string? digits = digits;
  public FloatExponent? exponent = exponent;

  public override string GenerateSource()
  {
    return digits + exponent?.GenerateSource();
  }
}

public enum SpecialFloatNumType
{
  inf,
  NaN,
}

public class SpecialFloatNum(SpecialFloatNumType? type) : FloatNum
{
  public SpecialFloatNumType? type = type;

  public override string GenerateSource()
  {
    return type.ToString() ?? "";
  }
}

public enum FloatSuffix
{
  f32,
  f64,
}

public class FloatValue(char? sign = null, FloatNum? num = null, FloatSuffix? suffix = null)
  : NumberValue
{
  public char? sign = sign;
  public FloatNum? num = num;
  public FloatSuffix? suffix = suffix;

  public override string GenerateSource()
  {
    return sign + num?.GenerateSource() + suffix;
  }
}
#endregion

public static class NumberParser
{
  #region Unsigned
  public static TextParser<UnsignedValue> UnsignedBinary_Parser = Span.EqualTo("0b")
    .Try()
    .IgnoreThen(
      StringParser
        .DigitBinary_Parser.AsDigitString()
        .Select(value => new UnsignedValue(UnsignedPrefix.binary, value))
    );

  public static TextParser<UnsignedValue> UnsignedOctal_Parser = Span.EqualTo("0o")
    .Try()
    .IgnoreThen(
      StringParser
        .DigitOctal_Parser.AsDigitString()
        .Select(value => new UnsignedValue(UnsignedPrefix.octal, value))
    );

  public static TextParser<UnsignedValue> UnsignedHex_Parser = Span.EqualTo("0x")
    .Try()
    .IgnoreThen(
      StringParser
        .DigitHexDecimal_Parser.AsDigitString()
        .Select(value => new UnsignedValue(UnsignedPrefix.hex, value))
    );

  public static TextParser<UnsignedValue> UnsignedDecimal_Parser = StringParser
    .Digit_Parser.AsDigitString()
    .Select(value => new UnsignedValue(null, value));

  public static TextParser<UnsignedValue> Unsigned_Parser = Parse.OneOf(
    UnsignedBinary_Parser,
    UnsignedOctal_Parser,
    UnsignedHex_Parser,
    UnsignedDecimal_Parser
  );
  #endregion

  #region Integer
  public static TextParser<IntegerSuffix> IntegerSuffix_Parser =
    MoreParsers.EnumParser<IntegerSuffix>();

  public static TextParser<IntegerValue> Integer_Parser =
    from sign in Character.In('-', '+').OrNull()
    from unsigned in Unsigned_Parser
    from suffix in IntegerSuffix_Parser.OrNull()
    select new IntegerValue(sign, unsigned, suffix);
  #endregion

  #region Byte

  public static TextParser<ByteValue> EscapedByte_Parser =
    from escape in Character.EqualTo('\\')
    from content in Parse.OneOf(StringParser.EscapeByte_Parser, StringParser.EscapeUnicode_Parser)
    select new ByteValue(escape + content.Evaluate());

  public static TextParser<ByteValue> ByteAscii_Parser = Character.AnyChar.Select(
    c => new ByteValue(c.ToString())
  );

  public static TextParser<ByteValue> ByteContent_Parser = Parse.OneOf(
    EscapedByte_Parser,
    ByteAscii_Parser
  );
  public static TextParser<ByteValue> Byte_Parser = Character
    .EqualTo('b')
    .IgnoreThen(ByteContent_Parser.Between(Character.EqualTo('\'')));
  #endregion

  #region Float
  public static TextParser<string> FloatInt_Parser = StringParser
    .Digit_Parser.AsDigitString()
    .ThenIgnore(Parse.Not(Character.EqualTo('.')));
  public static TextParser<string> FloatStd_Parser =
    from before in StringParser.Digit_Parser.AsDigitString()
    from dot in Span.EqualTo(".").ThenIgnore(Parse.Not(Character.EqualTo('.')))
    from after in StringParser.Digit_Parser.AsDigitString()!.OptionalOrDefault()
    select before + dot + after;

  public static TextParser<string> FloatFrac_Parser =
    from c in Span.EqualTo('.').ThenIgnore(Parse.Not(Character.EqualTo('.')))
    from digits in StringParser.Digit_Parser.AsDigitString()
    select c + digits;

  public static TextParser<FloatExponent> FloatExp_Parser =
    from e in Character.In('e', 'E')
    from sign in Character.In('-', '+').OrNull()
    from leading in Character.EqualTo('_').AsString()!.OptionalOrDefault()
    from digits in StringParser.Digit_Parser.AsDigitString()
    select new FloatExponent(e, sign, leading + digits);

  public static TextParser<FloatNum> StandardFloatNum_Parser =
    from digits in Parse.OneOf(FloatInt_Parser.Try(), FloatStd_Parser, FloatFrac_Parser)
    from exponent in FloatExp_Parser!.OptionalOrDefault()
    select new StandardFloatNum(digits, exponent) as FloatNum;

  public static TextParser<FloatNum> SpecialFloatNum_Parser = MoreParsers
    .EnumParser<SpecialFloatNumType>()
    .Select(value => new SpecialFloatNum(value) as FloatNum);

  public static TextParser<FloatNum> FloatNum_Parser = Parse.OneOf(
    StandardFloatNum_Parser,
    SpecialFloatNum_Parser
  );

  public static TextParser<FloatValue> Float_Parser =
    from sign in Character.In('+', '-').OrNull()
    from num in FloatNum_Parser
    from suffix in MoreParsers.EnumParser<FloatSuffix>().OrNull()
    select new FloatValue(sign, num, suffix);

  public static TextParser<NumberValue> Number_Parser = Parse.OneOf(
    Integer_Parser
      .AsNumberValue()
      .ThenIgnore(
        Parse.Not(
          Parse.OneOf(
            MoreParsers.EnumParser<FloatSuffix>().Ignore(),
            Character.In('.', 'e', 'E').Ignore()
          )
        )
      )
      .Try(),
    Byte_Parser.Try().AsNumberValue(),
    Float_Parser.AsNumberValue()
  );
  #endregion

  #region Extensions
  public static TextParser<char> OrUnderscore(this TextParser<char> source)
  {
    return Parse.OneOf(source, Character.EqualTo('_'));
  }

  public static TextParser<string> AsDigitString(this TextParser<char> digits)
  {
    return digits.Concat(digits.OrUnderscore());
  }

  public static TextParser<UnsignedValue> AsUnsignedValue<T>(this TextParser<T> source)
    where T : UnsignedValue
  {
    return source.Select(x => x as UnsignedValue);
  }

  public static TextParser<NumberValue> AsNumberValue<T>(this TextParser<T> source)
    where T : NumberValue
  {
    return source.Select(x => x as NumberValue);
  }

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

  public static TextParser<NumberExpr> NumberExpr_Parser =
    from start in MoreParsers.Position
    from num in Number_Parser
    from end in MoreParsers.Position
    select NumberExprFromNumberValue(RonSpan.From(start, end), num);

  private static NumberExpr NumberExprFromNumberValue(RonSpan span, NumberValue value)
  {
    if (value is IntegerValue intVal)
    {
      return new NumberExpr
      {
        kind = intVal.sign == '-' ? NumberKind.NegativeInteger : NumberKind.Integer,
        span = span,
        raw = value.GenerateSource(),
      };
    }
    else if (value is SpecialFloatNum)
    {
      return new NumberExpr
      {
        kind = NumberKind.SpecialFloat,
        span = span,
        raw = value.GenerateSource(),
      };
    }
    else if (value is StandardFloatNum)
    {
      return new NumberExpr
      {
        kind = NumberKind.Float,
        span = span,
        raw = value.GenerateSource(),
      };
    }
    else
      throw new InvalidOperationException();
  }
}
