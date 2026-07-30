using System.Numerics;

public static class NumberUtil
{
  /// <summary>
  /// Convert string in any base to base 10.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="base"></param>
  /// <returns></returns>
  public static string ToBase10String(this string source, int @base)
  {
    source = source.Replace("_", "");
    if (@base == 10)
    {
      return source;
    }

    IEnumerable<char> characters = source.Reverse();
    BigInteger place = 1;
    BigInteger accumulator = 0;
    foreach (var c in characters)
    {
      accumulator += place * int.Parse(c.ToString(), System.Globalization.NumberStyles.HexNumber);
      place *= @base;
    }
    return accumulator.ToString();
  }
}
