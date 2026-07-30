using System.Runtime.InteropServices;
using CriusNyx.Util;

public enum SignKind
{
  Unsigned = 0,
  Signed = 1,
}

public enum DecimalKind
{
  Integer = 0,
  FloatingPoint = 1,
}

public enum BitDepth
{
  b8 = 8,
  b16 = 16,
  b32 = 32,
  b64 = 64,

  // C# considers a decimal to a float to be a downcast
  @decimal = 64 + 1,
}

public class TypeIsNotANumberException(Type type) : Exception
{
  public override string Message => $"Type {type.Name} is not a number type.";
}

public static class TypeUtil
{
  private static HashSet<Type> UnsignedIntegerTypes =
  [
    typeof(byte),
    typeof(uint),
    typeof(nuint),
    typeof(ulong),
  ];

  private static HashSet<Type> SignedIntegerTypes =
  [
    typeof(sbyte),
    typeof(int),
    typeof(nint),
    typeof(long),
  ];

  private static HashSet<Type> IntegerTypes = [.. UnsignedIntegerTypes, .. SignedIntegerTypes];

  private static HashSet<Type> UnsignedFloatTypes = [typeof(ushort)];

  private static HashSet<Type> SignedFloatTypes =
  [
    typeof(short),
    typeof(float),
    typeof(double),
    typeof(decimal),
  ];

  private static HashSet<Type> FloatingPointTypes = [.. UnsignedFloatTypes, .. SignedFloatTypes];

  private static HashSet<Type> NumericTypes = [.. IntegerTypes, .. FloatingPointTypes];

  public static bool IsNumber(this Type type) => NumericTypes.Contains(type);

  public static bool IsInteger(this Type type) => IntegerTypes.Contains(type);

  public static bool IsUnsignedInteger(this Type type) => UnsignedIntegerTypes.Contains(type);

  public static bool IsSignedInteger(this Type type) => SignedIntegerTypes.Contains(type);

  public static bool IsFloatingPoint(this Type type) => FloatingPointTypes.Contains(type);

  public static SignKind GetSignKind(this Type type)
  {
    if (UnsignedIntegerTypes.Contains(type) || UnsignedFloatTypes.Contains(type))
    {
      return SignKind.Unsigned;
    }
    if (SignedIntegerTypes.Contains(type) || SignedFloatTypes.Contains(type))
    {
      return SignKind.Signed;
    }
    throw new TypeIsNotANumberException(type);
  }

  public static DecimalKind GetDecimalKind(this Type type)
  {
    if (IntegerTypes.Contains(type))
    {
      return DecimalKind.Integer;
    }
    if (FloatingPointTypes.Contains(type))
    {
      return DecimalKind.FloatingPoint;
    }
    throw new TypeIsNotANumberException(type);
  }

  public static BitDepth GetBitDepth(this Type type)
  {
    if (type == typeof(byte) || type == typeof(sbyte))
    {
      return BitDepth.b8;
    }
    if (type == typeof(short) || type == typeof(ushort))
    {
      return BitDepth.b16;
    }
    if (type == typeof(int) || type == typeof(uint) || type == typeof(float))
    {
      return BitDepth.b32;
    }
    if (type == typeof(long) || type == typeof(double))
    {
      return BitDepth.b64;
    }
    if (type == typeof(decimal))
    {
      return BitDepth.@decimal;
    }
    if (type == typeof(nint))
    {
      return (BitDepth)(Marshal.SizeOf<nint>() * 8);
    }
    if (type == typeof(nuint))
    {
      return (BitDepth)(Marshal.SizeOf<nuint>() * 8);
    }
    throw new TypeIsNotANumberException(type);
  }

  public static bool IsSigned(this Type type)
  {
    return GetSignKind(type) == SignKind.Signed;
  }

  public static bool IsUnsigned(this Type type)
  {
    return GetSignKind(type) == SignKind.Unsigned;
  }

  public static bool NumberCanUpcastTo(this Type type, Type other)
  {
    if (!type.IsNumber() || !other.IsNumber())
    {
      return false;
    }
    if (type.GetSignKind() > other.GetSignKind())
    {
      return false;
    }
    if (type.GetDecimalKind() > other.GetDecimalKind())
    {
      return false;
    }
    if (type.GetBitDepth() > other.GetBitDepth())
    {
      return false;
    }
    return true;
  }

  public static Func<string, object>? GetNumberParser(this Type? type)
  {
    if (type?.IsNumber() ?? false)
    {
      var method = type.GetMethod("Parse", [typeof(string)]);
      if (method is not null)
      {
        return (str) => method.Invoke(null, [str])!;
      }
    }
    return null;
  }

  public static bool IsEnumerable(this Type type)
  {
    return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
  }

  public static bool IsDictionaryType(this Type type)
  {
    return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>);
  }

  public static Type? GetListType(this Type type)
  {
    if (type.IsArray)
    {
      return type.GetElementType();
    }
    if (type.IsEnumerable())
    {
      return type.GetGenericArguments().First();
    }
    var enumerableInterfaces = type.GetInterfaces()
      .Where(interfaceType => interfaceType.IsEnumerable())
      .ToArray();
    if (enumerableInterfaces.Length == 1)
    {
      return enumerableInterfaces.First().GetGenericArguments().First();
    }
    return null;
  }

  public static Type? GetDictionaryValueType(this Type type)
  {
    if (type.IsDictionaryType())
    {
      return type.GetGenericArguments().Skip(1).First();
    }
    var dictInterfaces = type.GetInterfaces()
      .Where(interfaceType => interfaceType.IsDictionaryType())
      .ToArray();
    if (dictInterfaces.Length == 1)
    {
      return dictInterfaces.First().GetGenericArguments().Skip(1).First();
    }
    return null;
  }
}
