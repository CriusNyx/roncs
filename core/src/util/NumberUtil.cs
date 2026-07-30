using System.Numerics;

public static class NumberUtil
{
  public static string ToDecimalString(this string source, int @base)
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
