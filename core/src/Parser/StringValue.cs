using CriusNyx.Util;
using Superpower;

namespace RonCS.AST;

/// <summary>
/// Represents part of a string.
/// </summary>
public interface StringContent
{
  /// <summary>
  /// Evaluate the element as a C# string.
  /// </summary>
  /// <returns></returns>
  string EvaluateString();

  /// <summary>
  /// Convert the element to a RON string.
  /// </summary>
  /// <returns></returns>
  string Serialize();
}

/// <summary>
/// AST element representing a string.
/// </summary>
/// <param name="content"></param>
[DebugPrint]
public class StringValue(params StringContent[] content) : RonElement
{
  /// <summary>
  /// The content of the RON string.
  /// </summary>
  [DebugField]
  public StringContent[] content = content;

  public string Evaluate() => content.Select(x => x.EvaluateString()).StringJoin();

  public override string RonPrint(RonPrintOptions options)
  {
    return '"' + content.Select(x => x.Serialize()).StringJoin() + '"';
  }
};

/// <summary>
/// Literal string content.
/// </summary>
/// <param name="value"></param>
[DebugPrint]
public class StringLit(string value) : StringContent
{
  [DebugField]
  public string value = value;

  public string EvaluateString()
  {
    return value;
  }

  public string Serialize()
  {
    return value;
  }
}

/// <summary>
/// Unicode escape content.
/// </summary>
/// <param name="source"></param>
[DebugPrint]
public class UnicodeEscape(string source) : StringContent
{
  /// <summary>
  /// RON source code.
  /// </summary>
  [DebugField]
  public string source = source;

  public string EvaluateString()
  {
    uint u = uint.Parse(source, System.Globalization.NumberStyles.HexNumber);
    if (u >= 0x10000 && u <= 0x10FFFF)
    {
      uint uPrime = u - 0x10000;
      uint high = (uPrime >> 10) + 0xD800;
      uint low = (uPrime & 0x3ff) + 0xDC00;
      return new string([(char)high, (char)low]);
    }
    else if (u <= 0xD7FF || (u >= 0xE000 && u <= 0xFFFF))
    {
      return new string([(char)u]);
    }
    throw new InvalidOperationException();
  }

  public string Serialize()
  {
    return $"\\u{{{source}}}";
  }
}

/// <summary>
/// Raw string.
/// </summary>
/// <param name="content"></param>
[DebugPrint]
public class StringRawContent(StringContent content) : StringContent
{
  [DebugField]
  public StringContent content = content;

  public string EvaluateString()
  {
    return content.EvaluateString();
  }

  public string Serialize()
  {
    return content.Serialize();
  }
}

/// <summary>
/// Raw string literal.
/// </summary>
/// <param name="source"></param>
[DebugPrint]
public class StringRawLit(string source) : StringContent
{
  [DebugField]
  public string source = source;

  public string EvaluateString()
  {
    return source;
  }

  public string Serialize()
  {
    return source;
  }
}
