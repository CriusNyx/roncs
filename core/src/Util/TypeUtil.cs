using System.Runtime.InteropServices;

namespace RonCS;

/// <summary>
/// Sign kind for type.
/// </summary>
internal enum SignKind
{
  Unsigned = 0,
  Signed = 1,
}

/// <summary>
/// Decimal kind for type.
/// </summary>
internal enum DecimalKind
{
  Integer = 0,
  FloatingPoint = 1,
}

/// <summary>
/// Bit depth for type.
/// </summary>
internal enum BitDepth
{
  b8 = 8,
  b16 = 16,
  b32 = 32,
  b64 = 64,

  // C# considers a decimal to a float to be a downcast
  @decimal = 64 + 1,
}

/// <summary>
/// The designated type is not a number.
/// </summary>
/// <param name="type"></param>
internal class TypeIsNotANumberException(Type type) : Exception
{
  public override string Message => $"Type {type.Name} is not a number type.";
}

/// <summary>
/// Type extension methods.
/// </summary>
internal static class TypeUtil
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

  /// <summary>
  /// Returns true if the type is a number type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static bool IsNumber(this Type type) => NumericTypes.Contains(type);

  /// <summary>
  /// Returns true if the type is an integer type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static bool IsInteger(this Type type) => IntegerTypes.Contains(type);

  /// <summary>
  /// Returns true if the type is an unsigned integer type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static bool IsUnsignedInteger(this Type type) => UnsignedIntegerTypes.Contains(type);

  /// <summary>
  /// Returns true if the type is a signed integer type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static bool IsSignedInteger(this Type type) => SignedIntegerTypes.Contains(type);

  /// <summary>
  /// Returns true if the type is a floating point type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static bool IsFloatingPoint(this Type type) => FloatingPointTypes.Contains(type);

  /// <summary>
  /// Returns the signed kind for the type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  /// <exception cref="TypeIsNotANumberException"></exception>
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

  /// <summary>
  /// Returns the decimal kind for the type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  /// <exception cref="TypeIsNotANumberException"></exception>
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

  /// <summary>
  /// Returns the bit depth for the type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  /// <exception cref="TypeIsNotANumberException"></exception>
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

  /// <summary>
  /// Returns true if the type is signed.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static bool IsSigned(this Type type)
  {
    return GetSignKind(type) == SignKind.Signed;
  }

  /// <summary>
  /// Returns true if the type is unsigned.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static bool IsUnsigned(this Type type)
  {
    return GetSignKind(type) == SignKind.Unsigned;
  }

  /// <summary>
  /// Returns true if the source type can upcast to the target type.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="target"></param>
  /// <returns></returns>
  public static bool NumberCanUpcastTo(this Type source, Type target)
  {
    if (!source.IsNumber() || !target.IsNumber())
    {
      return false;
    }
    if (source.GetSignKind() > target.GetSignKind())
    {
      return false;
    }
    if (source.GetDecimalKind() > target.GetDecimalKind())
    {
      return false;
    }
    if (source.GetBitDepth() > target.GetBitDepth())
    {
      return false;
    }
    return true;
  }

  /// <summary>
  /// Get a function that parses the type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
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

  /// <summary>
  /// If the type is a generic type then convert it to it's general form. Otherwise returns the original type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static Type MakeGeneral(this Type type)
  {
    return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
  }

  /// <summary>
  /// Returns true if the type is an Enumerable type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static bool IsEnumerable(this Type type)
  {
    return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
  }

  /// <summary>
  /// Returns true if the type is a list type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static bool IsList(this Type type)
  {
    return type.MakeGeneral() == typeof(List<>);
  }

  /// <summary>
  /// Returns true if the type is a dictionary type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static bool IsDictionaryType(this Type type)
  {
    return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>);
  }

  /// <summary>
  /// Returns true if the type is a list type.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static bool IsListType(this Type type)
  {
    return type.IsArray || type.IsEnumerable() || type.IsList();
  }

  /// <summary>
  /// Gets the type of the list elements.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
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

  /// <summary>
  /// Gets the type of the dictionary elements value.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
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
