namespace RonCS.AST;

/// <summary>
/// AST element for an integer value.
/// </summary>
/// <param name="sign"></param>
/// <param name="digits"></param>
/// <param name="integerSuffix"></param>
public class RonInteger(
  char? sign = null,
  RonUnsigned? digits = null,
  IntegerSuffix? integerSuffix = null
) : RonElement, INumberValue
{
  /// <summary>
  /// Integer sign, is provided.
  /// </summary>
  public char? sign = sign;

  /// <summary>
  /// The digits for the number.
  /// </summary>
  public RonUnsigned? digits = digits;

  /// <summary>
  /// The suffix for the number.
  /// </summary>
  public IntegerSuffix? integerSuffix = integerSuffix;

  /// <inheritdoc/>
  public string ValueString()
  {
    return sign + digits?.ValueString();
  }

  /// <inheritdoc/>
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

  Type? INumberValue.CSType()
  {
    return integerSuffix.CSType();
  }

  /// <inheritdoc/>
  public override string RonPrint(RonPrintOptions options)
  {
    return $"{sign}{digits?.Serialize()}{integerSuffix}";
  }
}

/// <summary>
/// Suffix for an integer, indicating the bit width and sign.
/// </summary>
public enum IntegerSuffix
{
  /// <summary>
  /// Signed 8 bit integer
  /// </summary>
  i8,

  /// <summary>
  /// Signed 16 bit integer
  /// </summary>
  i16,

  /// <summary>
  /// Signed 32 bit integer
  /// </summary>
  i32,

  /// <summary>
  /// Signed 64 bit integer
  /// </summary>
  i64,

  /// <summary>
  /// Signed 128 bit integer
  /// </summary>
  i128,

  /// <summary>
  /// Unsigned 8 bit integer
  /// </summary>
  u8,

  /// <summary>
  /// Unsigned 16 bit integer
  /// </summary>
  u16,

  /// <summary>
  /// Unsigned 32 bit integer
  /// </summary>
  u32,

  /// <summary>
  /// Unsigned 64 bit integer
  /// </summary>
  u64,

  /// <summary>
  /// Unsigned 128 bit integer
  /// </summary>
  u128,
}
