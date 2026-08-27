using CriusNyx.Util;
using RonCS.Exceptions;

namespace RonCS.AST;

public interface NumberValue
{
  /// <summary>
  /// String representing the value of the number.
  /// Used for C# type conversion.
  /// </summary>
  /// <returns></returns>
  public string ValueString();

  /// <summary>
  /// Convert the value to a number.
  /// </summary>
  /// <param name="hint"></param>
  /// <returns></returns>
  public object EvaluateNumber(Type? hint);

  /// <summary>
  /// CS type for the element if it is unambiguous.
  /// </summary>
  /// <returns></returns>
  public Type? CSType();
}

#region Unsigned Value
/// <summary>
/// Unsigned integer value.
/// </summary>
/// <param name="prefix"></param>
/// <param name="digits"></param>
public class UnsignedValue(UnsignedPrefix? prefix = null, string? digits = null) : NumberValue
{
  /// <summary>
  /// Unsigned prefix value.
  /// </summary>
  public UnsignedPrefix? prefix = prefix;

  /// <summary>
  /// The digits for the unsigned value.
  /// </summary>
  public string? digits = digits;

  public string ValueString()
  {
    return digits?.ToBase10String(GetBase()) ?? "";
  }

  /// <summary>
  /// Get the base for the number.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  private int GetBase()
  {
    switch (prefix)
    {
      case UnsignedPrefix.binary:
        return 2;
      case UnsignedPrefix.octal:
        return 8;
      case null:
        return 10;
      case UnsignedPrefix.hex:
        return 16;
      default:
        throw new InvalidOperationException();
    }
  }

  public object EvaluateNumber(Type? hint)
  {
    throw new InvalidOperationException();
  }

  public Type? CSType()
  {
    throw RonException.CreateNotImplemented(nameof(CSType));
  }

  /// <summary>
  /// Convert the number to a Ron string.
  /// </summary>
  /// <returns></returns>
  public string Serialize()
  {
    return $"{prefix}{digits}";
  }
}

/// <summary>
/// Prefix for unsigned number, indicating the base.
/// </summary>
public enum UnsignedPrefix
{
  binary = 2,
  octal = 8,
  hex = 16,
}

#endregion

#region Integer Value
/// <summary>
/// Suffix for an integer, indicating the bit width and sign.
/// </summary>
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

/// <summary>
/// AST element for an integer value.
/// </summary>
/// <param name="sign"></param>
/// <param name="digits"></param>
/// <param name="integerSuffix"></param>
public class IntegerValue(
  char? sign = null,
  UnsignedValue? digits = null,
  IntegerSuffix? integerSuffix = null
) : RonElement, NumberValue
{
  /// <summary>
  /// Integer sign, is provided.
  /// </summary>
  public char? sign = sign;

  /// <summary>
  /// The digits for the number.
  /// </summary>
  public UnsignedValue? digits = digits;

  /// <summary>
  /// The suffix for the number.
  /// </summary>
  public IntegerSuffix? integerSuffix = integerSuffix;

  public string ValueString()
  {
    return sign + digits?.ValueString();
  }

  public object EvaluateNumber(Type? hint)
  {
    var parseType = hint ?? integerSuffix.CSType();
    if (parseType == null)
    {
      throw new InvalidOperationException();
    }
    var parser = parseType.GetNumberParser() ?? integerSuffix.CSType().GetNumberParser();
    if (parser == null)
    {
      throw new InvalidOperationException();
    }
    return parser?.Invoke(ValueString()).NotNull()!;
  }

  public Type? CSType()
  {
    return integerSuffix.CSType();
  }

  public override string RonPrint(RonPrintOptions options)
  {
    return $"{sign}{digits?.Serialize()}{integerSuffix}";
  }
}

#endregion

#region Float Value
/// <summary>
/// AST element for a float exponent.
/// </summary>
/// <param name="e"></param>
/// <param name="sign"></param>
/// <param name="digits"></param>
public class FloatExponent(char? e, char? sign, string? digits)
{
  /// <summary>
  /// Exponent character if provided
  /// </summary>
  public char? e = e;

  /// <summary>
  /// Sign.
  /// </summary>
  public char? sign = sign;

  /// <summary>
  /// Digits
  /// </summary>
  public string? digits = digits;

  /// <summary>
  /// Get a value string representing the number in C#.
  /// </summary>
  /// <returns></returns>
  public string ValueString()
  {
    return $"{e}{sign}{digits}";
  }

  /// <summary>
  /// Convert the element to a Ron string.
  /// </summary>
  /// <returns></returns>
  public string Serialize()
  {
    return $"e{sign}{digits}";
  }
}

/// <summary>
/// Base class for differentiating standard and special float numbers.
/// </summary>
public abstract class FloatNum
{
  /// <summary>
  /// Get a string representing the value of the element.
  /// </summary>
  /// <returns></returns>
  public abstract string ValueString();

  /// <summary>
  /// Convert the element to a Ron string.
  /// </summary>
  /// <returns></returns>
  public abstract string Serialize();
};

/// <summary>
/// AST element representing a standard float number.
/// </summary>
/// <param name="digits"></param>
/// <param name="exponent"></param>
public class StandardFloatNum(string? digits, FloatExponent? exponent) : FloatNum
{
  /// <summary>
  /// The digits of the float, if provided.
  /// </summary>
  public string? digits = digits;

  /// <summary>
  /// The exponent of the float, if provided.
  /// </summary>
  public FloatExponent? exponent = exponent;

  public override string ValueString()
  {
    return digits + exponent?.ValueString();
  }

  public override string Serialize()
  {
    return $"{digits}{exponent?.Serialize()}";
  }
}

/// <summary>
/// Type of a special float.
/// </summary>
public enum SpecialFloatNumType
{
  inf,
  NaN,
}

/// <summary>
/// AST element for a special float number.
/// </summary>
/// <param name="type"></param>
public class SpecialFloatNum(SpecialFloatNumType? type) : FloatNum
{
  /// <summary>
  /// The type of the float.
  /// </summary>
  public SpecialFloatNumType? type = type;

  public override string Serialize()
  {
    return type.NotNull(nameof(type)).ToString().NotNull(nameof(type));
  }

  public override string ValueString()
  {
    return type?.ToString() ?? "";
  }
}

/// <summary>
/// The suffix of a float, indicating the bit size.
/// </summary>
public enum FloatSuffix
{
  f32,
  f64,
}

/// <summary>
/// AST element representing a float value.
/// </summary>
/// <param name="sign"></param>
/// <param name="num"></param>
/// <param name="suffix"></param>
public class FloatValue(char? sign = null, FloatNum? num = null, FloatSuffix? suffix = null)
  : RonElement,
    NumberValue
{
  /// <summary>
  /// Sign of the float
  /// </summary>
  public char? sign = sign;

  /// <summary>
  /// The number value of the float.
  /// </summary>
  public FloatNum? num = num;

  /// <summary>
  /// The suffix of the float.
  /// </summary>
  public FloatSuffix? suffix = suffix;

  public string ValueString()
  {
    return sign + num?.ValueString();
  }

  /// <summary>
  /// Evaluate the float if it is a special number type.
  /// </summary>
  /// <param name="specialNum"></param>
  /// <param name="hint"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  private object EvaluateSpecialNum(SpecialFloatNum specialNum, Type? hint)
  {
    if (specialNum.type == SpecialFloatNumType.inf)
    {
      if (hint == typeof(float))
      {
        if (sign == '-')
        {
          return float.NegativeInfinity;
        }
        else
        {
          return float.PositiveInfinity;
        }
      }
      else
      {
        if (sign == '-')
        {
          return double.NegativeInfinity;
        }
        else
        {
          return double.PositiveInfinity;
        }
      }
    }
    else if (specialNum.type == SpecialFloatNumType.NaN)
    {
      if (hint == typeof(float))
      {
        return float.NaN;
      }
      else
      {
        return double.NaN;
      }
    }
    throw new InvalidOperationException();
  }

  /// <summary>
  /// Evaluate the number into a C# type.
  /// </summary>
  /// <param name="hint"></param>
  /// <returns></returns>
  public object EvaluateNumber(Type? hint)
  {
    if (num is SpecialFloatNum specialNum)
    {
      return EvaluateSpecialNum(specialNum, hint);
    }
    var csType = suffix.CSType() ?? hint;
    var parser = csType.GetNumberParser().NotNull();
    return parser?.Invoke(ValueString()).NotNull()!;
  }

  public Type? CSType()
  {
    return suffix.CSType();
  }

  public override string RonPrint(RonPrintOptions options)
  {
    return $"{sign}{num?.Serialize()}{suffix}";
  }
}
#endregion


public static class NumberValueExtensions
{
  /// <summary>
  /// Get the CS type for the number suffix if it is unambiguous.
  /// </summary>
  /// <param name="suffix"></param>
  /// <returns></returns>
  public static Type? CSType(this IntegerSuffix? suffix)
  {
    return suffix switch
    {
      IntegerSuffix.i8 => typeof(sbyte),
      IntegerSuffix.i16 => typeof(short),
      IntegerSuffix.i32 => typeof(int),
      IntegerSuffix.i64 => typeof(long),
      IntegerSuffix.i128 => typeof(Int128),
      IntegerSuffix.u8 => typeof(byte),
      IntegerSuffix.u16 => typeof(ushort),
      IntegerSuffix.u32 => typeof(uint),
      IntegerSuffix.u64 => typeof(ulong),
      IntegerSuffix.u128 => typeof(UInt128),
      null => null,
      _ => throw UnknownEnumException.Create(suffix),
    };
  }

  /// <summary>
  /// Get the C# type if it is unambiguous.
  /// </summary>
  /// <param name="suffix"></param>
  /// <returns></returns>
  public static Type? CSType(this FloatSuffix? suffix)
  {
    return suffix switch
    {
      FloatSuffix.f32 => typeof(float),
      FloatSuffix.f64 => typeof(double),
      null => null,
      _ => throw UnknownEnumException.Create(suffix),
    };
  }
}
