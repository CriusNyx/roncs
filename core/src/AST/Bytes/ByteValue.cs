using RonCS.Exceptions;

namespace RonCS.AST;

/// <summary>
/// AST element for a byte value.
/// </summary>
/// <param name="inner"></param>
public class RonByte(INumberValue inner) : RonElement, INumberValue
{
  /// <summary>
  /// The inner value of the byte.
  /// </summary>
  public INumberValue inner = inner;

  /// <inheritdoc/>
  public string ValueString()
  {
    return inner.ValueString();
  }

  /// <inheritdoc/>
  public object EvaluateNumber(Type? hint)
  {
    return inner.EvaluateNumber(hint);
  }

  Type? INumberValue.CSType()
  {
    return inner.CSType();
  }

  /// <inheritdoc/>
  public override string RonPrint(RonPrintOptions options)
  {
    throw RonException.CreateNotImplemented(nameof(RonPrint), options);
  }
}
