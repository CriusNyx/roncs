namespace RonCS.AST;

/// <summary>
/// Unicode escape content.
/// </summary>
/// <param name="source"></param>
public class RonUnicodeEscape(string source) : StringContent
{
  /// <summary>
  /// RON source code.
  /// </summary>
  public string source = source;

  /// <inheritdoc/>
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

  /// <inheritdoc/>
  public string Serialize()
  {
    return $"\\u{{{source}}}";
  }
}
