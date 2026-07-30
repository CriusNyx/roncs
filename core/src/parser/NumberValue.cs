using CriusNyx.Util;

namespace RonCS;

public abstract class NumberValue : RonElement
{
  public abstract string ValueString();
  public abstract object Parse(Type? hint);
  public abstract Type? CSType();
}

#region Unsigned Value
public class UnsignedValue(UnsignedPrefix? prefix = null, string? digits = null) : NumberValue
{
  public UnsignedPrefix? prefix = prefix;
  public string? digits = digits;

  public override string ValueString()
  {
    return digits?.ToDecimalString(GetBase()) ?? "";
  }

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

  public override object Parse(Type? hint)
  {
    throw new InvalidOperationException();
  }

  public override Type? CSType()
  {
    throw new NotImplementedException();
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

  public override string ValueString()
  {
    return sign + digits?.ValueString();
  }

  public override object Parse(Type? hint)
  {
    var parseType = hint ?? integerSuffix.CSType();
    if (parseType == null)
    {
      throw new InvalidOperationException();
    }
    var parser = parseType.GetNumberParser();
    if (parser == null)
    {
      throw new InvalidOperationException();
    }
    return parser?.Invoke(ValueString()).NotNull()!;
  }

  public override Type? CSType()
  {
    return integerSuffix.CSType();
  }
}
#endregion

#region Byte Value
public class ByteValue(string source) : NumberValue
{
  public string source = source;

  public override string ValueString()
  {
    // TODO: I think this is wrong.
    return source;
  }

  public override object Parse(Type? hint)
  {
    throw new NotImplementedException();
  }

  public override Type? CSType()
  {
    return typeof(byte);
  }
}
#endregion

#region Float Value
public class FloatExponent(char? e, char? sign, string? digits)
{
  public char? e = e;
  public char? sign = sign;
  public string? digits = digits;

  public string ValueString()
  {
    return $"{e}{sign}{digits}";
  }
}

public abstract class FloatNum
{
  public abstract string ValueString();
};

public class StandardFloatNum(string? digits, FloatExponent? exponent) : FloatNum
{
  public string? digits = digits;
  public FloatExponent? exponent = exponent;

  public override string ValueString()
  {
    return digits + exponent?.ValueString();
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

  public override string ValueString()
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

  public override string ValueString()
  {
    return sign + num?.ValueString();
  }

  public override object Parse(Type? hint)
  {
    var csType = suffix.CSType() ?? hint;
    var parser = csType.GetNumberParser().NotNull();
    return parser?.Invoke(ValueString()).NotNull()!;
  }

  public override Type? CSType()
  {
    return suffix?.GetType();
  }
}
#endregion


public static class NumberValueExtensions
{
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

  public static string CSPrefix(this UnsignedPrefix? prefix)
  {
    switch (prefix)
    {
      case UnsignedPrefix.binary:
        return "0b";
      case UnsignedPrefix.hex:
        return "0x";
      // CS Does not have an octal prefix. :'(
      // I will have to do a base conversion on the source string.
      case UnsignedPrefix.octal:
        throw new InvalidOperationException();
      case null:
        return "";
      default:
        throw new InvalidCastException();
    }
  }
}
