using System.Numerics;
using RonCS.Exceptions;

namespace RonCS.AST;

/// <summary>
/// Prefix for unsigned number, indicating the base.
/// </summary>
public enum UnsignedPrefix
{
  /// <summary>
  /// 0b
  /// </summary>
  binary = 2,

  /// <summary>
  /// 0o
  /// </summary>
  octal = 8,

  /// <summary>
  /// 0x
  /// </summary>
  hex = 16,
}

/// <summary>
/// Unsigned integer value.
/// </summary>
/// <param name="prefix"></param>
/// <param name="digits"></param>
public class RonUnsigned(UnsignedPrefix? prefix = null, string? digits = null) : INumberValue
{
  /// <summary>
  /// Unsigned prefix value.
  /// </summary>
  public UnsignedPrefix? prefix = prefix;

  /// <summary>
  /// The digits for the unsigned value.
  /// </summary>
  public string? digits = digits;

  /// <inheritdoc/>
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

  /// <inheritdoc/>
  public object EvaluateNumber(Type? hint)
  {
    throw new InvalidOperationException();
  }

  Type? INumberValue.CSType()
  {
    throw RonException.CreateNotImplemented(nameof(INumberValue.CSType));
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
