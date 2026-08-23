using System.Text.RegularExpressions;
using CriusNyx.Util;
using DeepEqual.Syntax;
using RonCS;
using RonCS.AST;
using RonCS.Exceptions;
using Superpower;

namespace RonTests;

public class NumberParserTests
{
  public static IEnumerable<(char?, UnsignedPrefix?, string, IntegerSuffix?)> IntegerTestCases
  {
    get
    {
      foreach (var sign in new char?[] { null, '-', '+' })
      foreach (
        var prefix in Enum.GetValues<UnsignedPrefix>().WhereAs<UnsignedPrefix?>().Concat([null])
      )
      foreach (var digits in new string?[] { "0", "1", "9", "A", "a" })
      foreach (
        var suffix in Enum.GetValues<IntegerSuffix>().WhereAs<IntegerSuffix?>().Concat([null])
      )
      {
        yield return (sign, prefix, digits, suffix)!;
      }
    }
  }

  [Test]
  public void IntegerParsesCorrectly()
  {
    foreach (var (sign, prefix, digits, suffix) in IntegerTestCases)
    {
      string source = $"{sign}{prefix.ToPrefixString()}{digits}{suffix}";
      var parser = NumberParser.Number_Parser.AtEnd();

      if (GetIntegerBase(digits) > GetIntegerBase(prefix))
      {
        Assert.Throws<ParseException>(() => parser.Parse(source));
      }
      else
      {
        var expected = new IntegerValue(sign, new(prefix, digits), suffix);
        var actual = parser.Parse(source);
        actual.ShouldDeepEqual(expected);
      }
    }
  }

  public static IEnumerable<(
    char?,
    string?,
    bool,
    string?,
    char?,
    char?,
    string?,
    FloatSuffix?
  )> StandardFloatParsesCorrectlyParams
  {
    get
    {
      foreach (var sign in new char?[] { '-', '+', null })
      foreach (var beforeDecimal in new string?[] { "0", "1", "9", null })
      foreach (var withDecimal in new bool[] { false, true })
      foreach (var afterDecimal in new string?[] { "0", "1", "9", null })
      foreach (var exponentChar in new char?[] { 'e', 'E', null })
      foreach (var exponentSign in new char?[] { '-', '+', null })
      foreach (var exponentDigits in new string?[] { "0", "1", "9", null })
      foreach (var suffix in new FloatSuffix?[] { FloatSuffix.f32, FloatSuffix.f64, null })
      {
        // There is no number.
        if (beforeDecimal == null && afterDecimal == null)
        {
          continue;
        }
        // If there is no decimal, and there is a before and after then this is not a float.
        if (withDecimal && beforeDecimal != null && afterDecimal != null)
        {
          continue;
        }
        // Filter out exponents with no exponent character.
        if (exponentChar == null && (exponentSign != null || exponentDigits != null))
        {
          continue;
        }
        // Invalid exponent.
        if (exponentChar != null && exponentDigits == null)
        {
          continue;
        }
        // Filter out integers.
        if (!withDecimal && suffix == null && exponentChar == null)
        {
          continue;
        }
        yield return (
          sign,
          beforeDecimal,
          withDecimal,
          afterDecimal,
          exponentChar,
          exponentSign,
          exponentDigits,
          suffix
        );
      }
    }
  }

  [Test]
  public void StandardFloatParsesCorrectly()
  {
    foreach (
      var (
        sign,
        digitsBeforeDecimal,
        decimalPoint,
        digitsAfterDecimal,
        exponentChar,
        exponentSign,
        exponentDigits,
        suffix
      ) in StandardFloatParsesCorrectlyParams
    )
    {
      string decChar = decimalPoint ? "." : "";

      string source =
        $"{sign}{digitsBeforeDecimal}{decChar}{digitsAfterDecimal}{exponentChar}{exponentSign}{exponentDigits}{suffix}";

      var expected = new FloatValue(
        sign,
        new StandardFloatNum(
          $"{digitsBeforeDecimal}{decChar}{digitsAfterDecimal}",
          exponentChar != null
            ? new FloatExponent(exponentChar, exponentSign, exponentDigits)
            : null
        ),
        suffix
      );

      var actual = NumberParser.Number_Parser.Parse(source);

      actual.ShouldDeepEqual(expected);
    }
  }

  public static IEnumerable<(
    char?,
    SpecialFloatNumType,
    FloatSuffix?
  )> SpecialFloatParsesCorrectlyData
  {
    get
    {
      foreach (var c in new char?[] { '-', '+', null })
      foreach (
        var specialType in new SpecialFloatNumType[]
        {
          SpecialFloatNumType.inf,
          SpecialFloatNumType.NaN,
        }
      )
      foreach (var suffix in new FloatSuffix?[] { FloatSuffix.f32, FloatSuffix.f64, null })
      {
        yield return (c, specialType, suffix);
      }
    }
  }

  [Test]
  public void SpecialFloatParsesCorrectly()
  {
    foreach (var (sign, type, suffix) in SpecialFloatParsesCorrectlyData)
    {
      string source = $"{sign}{type}{suffix}";
      var parsed = NumberParser.Number_Parser.Parse(source);
      var expected = new FloatValue(sign, new SpecialFloatNum(type), suffix);
      parsed.ShouldDeepEqual(expected);
    }
  }

  private static int GetIntegerBase(UnsignedPrefix? prefix)
  {
    if (prefix is not null)
    {
      return (int)prefix;
    }
    return 10;
  }

  private static int GetIntegerBase(string source)
  {
    if (Regex.IsMatch(source, "^[0-1]+$"))
    {
      return 2;
    }
    if (Regex.IsMatch(source, "^[0-7]+$"))
    {
      return 8;
    }
    if (Regex.IsMatch(source, "^[0-9]+$"))
    {
      return 10;
    }
    if (Regex.IsMatch(source, "^[0-9a-fA-F]+$"))
    {
      return 16;
    }
    throw RonException.CreateNotImplemented(nameof(GetIntegerBase));
  }
}
