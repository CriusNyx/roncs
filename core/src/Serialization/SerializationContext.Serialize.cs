using System.Text.RegularExpressions;
using RonCS.AST;
using RonCS.Exceptions;
using Superpower;

namespace RonCS;

public partial class SerializationContext
{
  /// <summary>
  /// Memoized converters to use.
  /// </summary>
  private static Dictionary<Type, TypeSerializerConverter> converterCache =
    new Dictionary<Type, TypeSerializerConverter>();

  /// <summary>
  /// Convert the object to a Ron AST.
  /// If field type is provided it will allow for inference for the element.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="fieldType"></param>
  /// <returns></returns>
  public RonElement ToAST(object? source, Type? fieldType = null)
  {
    if (source is null)
    {
      return new RonNone();
    }

    IntegerSuffix? IntType(IntegerSuffix suffix)
    {
      if (source.GetType() == fieldType)
      {
        return null;
      }
      return suffix;
    }

    FloatSuffix? FloatType(FloatSuffix suffix)
    {
      if (source.GetType() == fieldType)
      {
        return null;
      }
      return suffix;
    }

    return source switch
    {
      // Boolean
      bool value => new RonBool(value),

      // Strings
      char value => new RonChar(value),
      string str => new RonString([new RonStringLit(str)]),

      // Integers
      byte value => CreateIntValue(value, IntType(IntegerSuffix.u8)),
      sbyte value => CreateIntValue(value, IntType(IntegerSuffix.i8)),
      short value => CreateIntValue(value, IntType(IntegerSuffix.i16)),
      ushort value => CreateIntValue(value, IntType(IntegerSuffix.u16)),
      int value => CreateIntValue(value, IntType(IntegerSuffix.i32)),
      uint value => CreateIntValue(value, IntType(IntegerSuffix.u32)),
      nint value => CreateIntValue(value, null),
      nuint value => CreateIntValue(value, null),
      long value => CreateIntValue(value, IntType(IntegerSuffix.i64)),
      ulong value => CreateIntValue(value, IntType(IntegerSuffix.u64)),

      // Floats and decimals
      float value => CreateFloatValue(value, FloatType(FloatSuffix.f32)),
      double value => CreateFloatValue(value, FloatType(FloatSuffix.f64)),
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

  /// <summary>
  /// Create a Ron integer element.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="suffix"></param>
  /// <returns></returns>
  private static RonInteger CreateIntValue(object source, IntegerSuffix? suffix)
  {
    return new RonInteger(
      Convert.ToInt64(source) < 0 ? '-' : null,
      new(null, source.ToString().NotNull().Replace("-", "")),
      suffix
    );
  }

  /// <summary>
  /// Create a Ron float element.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="suffix"></param>
  /// <returns></returns>
  private static RonFloat CreateFloatValue(object source, FloatSuffix? suffix)
  {
    var asDouble = Convert.ToDouble(source);
    if (double.IsNaN(asDouble))
    {
      return new RonFloat(null, new RonSpecialFloat(SpecialFloatType.NaN), null);
    }
    char? sign = asDouble < 0 ? '-' : null;
    if (double.IsInfinity(asDouble))
    {
      return new RonFloat(sign, new RonSpecialFloat(SpecialFloatType.inf), null);
    }
    var str = ShortestRound(source);
    var output = NumberParser.Float_Parser.Parse(str.NotNull("str"));
    output.suffix = suffix;
    if (output.num is RonStandardFloat { exponent: { sign: '+' } } std)
    {
      std.exponent.sign = null;
    }
    return output;
  }

  /// <summary>
  /// Determine the shortest round trippable string.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
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
    throw RonException.CreateNotImplemented(nameof(ShortestRound));
  }
}
