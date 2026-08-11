using System.Text.RegularExpressions;
using CriusNyx.Util;
using Superpower;

namespace RonCS;

public partial class SerializationContext
{
  private static Dictionary<Type, TypeSerializerConverter> converterCache =
    new Dictionary<Type, TypeSerializerConverter>();

  public RonElement ToAST(object? source)
  {
    if (source is null)
    {
      return new RonNone();
    }

    return source switch
    {
      // Boolean
      bool value => new RonBool(value),

      // Strings
      char value => new RonChar(value),
      string str => new StringValue([new StringLit(str)]),

      // Integers
      byte value => CreateIntValue(value, IntegerSuffix.u8),
      sbyte value => CreateIntValue(value, IntegerSuffix.i8),
      short value => CreateIntValue(value, IntegerSuffix.i16),
      ushort value => CreateIntValue(value, IntegerSuffix.u16),
      int value => CreateIntValue(value, IntegerSuffix.i32),
      uint value => CreateIntValue(value, IntegerSuffix.u32),
      nint value => CreateIntValue(value, null),
      nuint value => CreateIntValue(value, null),
      long value => CreateIntValue(value, IntegerSuffix.i64),
      ulong value => CreateIntValue(value, IntegerSuffix.u64),

      // Floats and decimals
      float value => CreateFloatValue(value, FloatSuffix.f32),
      double value => CreateFloatValue(value, FloatSuffix.f64),
      decimal value => CreateFloatValue(value, null),

      // Objects
      object o => converterCache
        .GetOrSet(
          o.GetType(),
          () => TypeSerializerConverter.CreateTypeConverterForObjectType(o.GetType())
        )
        .ToAST(this, o),
    };
  }

  private static IntegerValue CreateIntValue(object source, IntegerSuffix? suffix)
  {
    return new IntegerValue(
      Convert.ToInt64(source) < 0 ? '-' : null,
      new(null, source.ToString().NotNull().Replace("-", "")),
      suffix
    );
  }

  private static FloatValue CreateFloatValue(object source, FloatSuffix? suffix)
  {
    var asDouble = Convert.ToDouble(source);
    if (double.IsNaN(asDouble))
    {
      return new FloatValue(null, new SpecialFloatNum(SpecialFloatNumType.NaN), null);
    }
    char? sign = asDouble < 0 ? '-' : null;
    if (double.IsInfinity(asDouble))
    {
      return new FloatValue(sign, new SpecialFloatNum(SpecialFloatNumType.inf), null);
    }
    var str = ShortestRound(source);
    var output = NumberParser.Float_Parser.Parse(str.NotNull("str"));
    output.suffix = suffix;
    if (output.num is StandardFloatNum { exponent: { sign: '+' } } std)
    {
      std.exponent.sign = null;
    }
    return output;
  }

  private static string ShortestRound(object source)
  {
    string Solve(string science)
    {
      if (!science.StartsWith('-'))
      {
        science = '+' + science;
      }
      var match = Regex.Match(science, "(\\+|-)(\\d*)\\.(\\d*)e([\\+|-]\\d*)");
      var (sign, before, after, exponent) = match
        .Groups.NotNull("Groups")
        .Cast<Group>()
        .Select(x => x.Value)
        .Skip(1)
        .Take<string, string, string, string>();

      // Remove plus sign
      if (sign == "+")
      {
        sign = "";
      }

      // Shorten after
      after = after.NotNull().TrimEnd('0');

      var afterPart = after == "" ? "" : $".{after}";

      // Shorten Exponent
      var eSign = exponent.NotNull().StartsWith('-') ? "-" : "";
      var eShortened = exponent.NotNull().TrimStart('-', '+', '0');

      var ePart = eShortened == "" ? "" : $"e{eSign}{eShortened}";

      return $"{sign}{before}{afterPart}{ePart}";
    }

    if (source is double d)
    {
      return Enumerable
        .MinBy([d.ToString(), Solve(d.ToString("e"))], x => x.NotNull().Length)
        .NotNull();
    }
    if (source is float f)
    {
      return Enumerable
        .MinBy([f.ToString(), Solve(f.ToString("e"))], x => x.NotNull().Length)
        .NotNull();
    }
    if (source is decimal dec)
    {
      return Enumerable
        .MinBy([dec.ToString(), Solve(dec.ToString("e"))], x => x.NotNull().Length)
        .NotNull();
    }
    throw new NotImplementedException();
  }

  public void RegisterTypeConverter(Type type, TypeSerializerConverter converter)
  {
    converterCache[type] = converter;
  }

  public void RegisterListType(Type type)
  {
    converterCache[type] = new ListConverter();
  }
}
