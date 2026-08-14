using CriusNyx.Util;
using Superpower;
using Superpower.Parsers;

namespace RonCS;

public static class ParseExtensions
{
  public static TextParser<T> ThenIgnore<T, U>(this TextParser<T> self, TextParser<U> other)
  {
    return self.Then((value) => other.Value(value));
  }

  public static TokenListParser<TKind, T> ThenIgnore<TKind, T, U>(
    this TokenListParser<TKind, T> self,
    TokenListParser<TKind, U> other
  )
  {
    return self.Then((value) => other.Value(value));
  }

  public static TokenListParser<TKind, IEnumerable<T>> SeparatedBy<TKind, T, U>(
    this TokenListParser<TKind, T> self,
    TokenListParser<TKind, U> separator
  )
  {
    return from start in self
      from rest in separator.IgnoreThen(self).Try().Many()
      select start.ThenConcat(rest);
  }

  public static TokenListParser<TKind, T> Between<TKind, T, U, V>(
    this TokenListParser<TKind, T> self,
    TokenListParser<TKind, U> before,
    TokenListParser<TKind, V> after
  )
  {
    return before.IgnoreThen(self).ThenIgnore(after);
  }

  public static TextParser<T> Between<T, U>(this TextParser<T> self, TextParser<U> other)
  {
    return self.Between(other, other);
  }

  public static TokenListParser<TKind, T> Between<TKind, T, U, V>(
    this TokenListParser<TKind, T> self,
    TokenListParser<TKind, U> other
  )
  {
    return self.Between(other, other);
  }

  public static TextParser<T> EnumParser<T>()
    where T : struct, Enum
  {
    return Parse.OneOf(
      Enum.GetValues<T>()
        .Select(value => Span.EqualTo(value.ToString()).Value(value).Try())
        .ToArray()
    );
  }

  public static TextParser<string> StringIn(params string[] source)
  {
    return Parse.OneOf(source.Select(source => Span.EqualTo(source).Try().AsString()).ToArray());
  }

  public static TextParser<string> AsString(
    this TextParser<char> charParser,
    bool atLeastOnce = false
  )
  {
    if (atLeastOnce)
    {
      return charParser.AtLeastOnce().Select(chars => new string(chars));
    }
    else
    {
      return charParser.Many().Select(chars => new string(chars));
    }
  }

  public static TextParser<IEnumerable<T>> UpTo<T>(this TextParser<T> source, int value)
  {
    if (value == 1)
    {
      return source.Select(x => new T[] { x } as IEnumerable<T>);
    }
    else
      return from self in source
        from other in source.UpTo(value - 1).OptionalOrDefault([])
        select self.ThenConcat(other);
  }
}
