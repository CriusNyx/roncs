using CriusNyx.Util;
using Superpower;

namespace RonCS;

public interface StringContent
{
  string EvaluateString();
  string Serialize();
}

[DebugPrint]
public class StringValue(params StringContent[] content) : RonElement
{
  [DebugField]
  public StringContent[] content = content;

  public string Evaluate() => content.Select(x => x.EvaluateString()).StringJoin();

  public override string RonPrint(RonPrintOptions options)
  {
    return '"' + content.Select(x => x.Serialize()).StringJoin() + '"';
  }
};

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

[DebugPrint]
public class UnicodeEscape(string source) : StringContent
{
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
