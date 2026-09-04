using RonCS.Exceptions;

namespace RonCS.AST;

#region Float Value
#endregion


internal static class NumberValueExtensions
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
