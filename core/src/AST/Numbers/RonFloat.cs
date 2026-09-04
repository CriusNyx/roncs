namespace RonCS.AST;

/// <summary>
/// The suffix of a float, indicating the bit size.
/// </summary>
public enum FloatSuffix
{
  /// <summary>
  /// 32 bit floating point number
  /// </summary>
  f32,

  /// <summary>
  /// 64 bit floating point number.
  /// </summary>
  f64,
}

/// <summary>
/// AST element representing a float value.
/// </summary>
/// <param name="sign"></param>
/// <param name="num"></param>
/// <param name="suffix"></param>
public class RonFloat(char? sign = null, RonFloatNumber? num = null, FloatSuffix? suffix = null)
  : RonElement,
    INumberValue
{
  /// <summary>
  /// Sign of the float
  /// </summary>
  public char? sign = sign;

  /// <summary>
  /// The number value of the float.
  /// </summary>
  public RonFloatNumber? num = num;

  /// <summary>
  /// The suffix of the float.
  /// </summary>
  public FloatSuffix? suffix = suffix;

  /// <inheritdoc/>
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
  private object EvaluateSpecialNum(RonSpecialFloat specialNum, Type? hint)
  {
    if (specialNum.type == SpecialFloatType.inf)
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
    else if (specialNum.type == SpecialFloatType.NaN)
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
    if (num is RonSpecialFloat specialNum)
    {
      return EvaluateSpecialNum(specialNum, hint);
    }
    var csType = suffix.CSType() ?? hint;
    var parser = csType.GetNumberParser().NotNull();
    return parser?.Invoke(ValueString()).NotNull()!;
  }

  Type? INumberValue.CSType()
  {
    return suffix.CSType();
  }

  /// <inheritdoc/>
  public override string RonPrint(RonPrintOptions options)
  {
    return $"{sign}{num?.Serialize()}{suffix}";
  }
}
