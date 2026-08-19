using CriusNyx.Util;

namespace RonCS;

public class ByteValue(NumberValue inner) : RonElement, NumberValue
{
  public NumberValue inner = inner;

  public string ValueString()
  {
    return inner.ValueString();
  }

  public object EvaluateNumber(Type? hint)
  {
    return inner.EvaluateNumber(hint);
  }

  public Type? CSType()
  {
    return inner.CSType();
  }

  public override string RonPrint(RonPrintOptions options)
  {
    throw RonException.CreateNotImplemented(nameof(RonPrint), options);
  }
}

[DebugPrint]
public class AsciiEscape(char source) : StringContent, NumberValue
{
  [DebugField]
  public char? source = source;

  public Type? CSType()
  {
    return typeof(byte);
  }

  public object EvaluateNumber(Type? hint)
  {
    if (source is char c)
    {
      return (byte)c;
    }
    throw new NullReferenceException("source is null.");
  }

  public string EvaluateString()
  {
    return source switch
    {
      '\'' => "'",
      '"' => "\"",
      '\\' => "\\",
      'n' => "\n",
      'r' => "\r",
      't' => "\t",
      '0' => "\0",
      _ => throw RonException.CreateNotImplemented(nameof(EvaluateString)),
    };
  }

  public string Serialize()
  {
    return $"\\{source}";
  }

  public string ValueString()
  {
    throw RonException.CreateNotImplemented(nameof(ValueString));
  }
}

[DebugPrint]
public class ByteEscape(char left, char right) : StringContent, NumberValue
{
  [DebugField]
  public char? left = left;

  [DebugField]
  public char? right = right;

  public Type? CSType()
  {
    return typeof(byte);
  }

  public object EvaluateNumber(Type? hint)
  {
    return byte.Parse($"{left}{right}", System.Globalization.NumberStyles.HexNumber);
  }

  public string EvaluateString()
  {
    char l = (char)left.NotNull(nameof(left))!;
    char r = (char)right.NotNull(nameof(right))!;
    var b = byte.Parse([l, r], System.Globalization.NumberStyles.HexNumber);
    return ((char)b).ToString();
  }

  public string Serialize()
  {
    return $"x{left}{right}";
  }

  public string ValueString()
  {
    throw RonException.CreateNotImplemented(nameof(ValueString));
  }
}

public class AsciiLiteral(char c) : NumberValue
{
  public Type? CSType()
  {
    return typeof(byte);
  }

  public object EvaluateNumber(Type? hint)
  {
    return (byte)c;
  }

  public string ValueString()
  {
    return c.ToString();
  }
}
