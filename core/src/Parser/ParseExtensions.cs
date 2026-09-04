using RonCS.AST;
using Superpower;
using Superpower.Parsers;

namespace RonCS.AST;

/// <summary>
/// Extensions for Superpower parsers.
/// </summary>
internal static class ParseExtensions
{
  /// <summary>
  /// Try self, and then other. If both succeed then return the value of self.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="U"></typeparam>
  /// <param name="self"></param>
  /// <param name="other"></param>
  /// <returns></returns>
  public static TextParser<T> ThenIgnore<T, U>(this TextParser<T> self, TextParser<U> other)
  {
    return self.Then((value) => other.Value(value));
  }

  /// <summary>
  /// Parse self, then other, returning the result of self.
  /// </summary>
  /// <typeparam name="TKind"></typeparam>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="U"></typeparam>
  /// <param name="self"></param>
  /// <param name="other"></param>
  /// <returns></returns>
  public static TokenListParser<TKind, T> ThenIgnore<TKind, T, U>(
    this TokenListParser<TKind, T> self,
    TokenListParser<TKind, U> other
  )
  {
    return self.Then((value) => other.Value(value));
  }

  /// <summary>
  /// Parse any number of element separated by the separator element.
  /// </summary>
  /// <typeparam name="TKind"></typeparam>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="U"></typeparam>
  /// <param name="self"></param>
  /// <param name="separator"></param>
  /// <returns></returns>
  public static TokenListParser<TKind, IEnumerable<T>> SeparatedBy<TKind, T, U>(
    this TokenListParser<TKind, T> self,
    TokenListParser<TKind, U> separator
  )
  {
    return from start in self
      from rest in separator.IgnoreThen(self).Try().Many()
      select start.ThenConcat(rest);
  }

  /// <summary>
  /// Parse elements between before and after.
  /// </summary>
  /// <typeparam name="TKind"></typeparam>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="U"></typeparam>
  /// <typeparam name="V"></typeparam>
  /// <param name="self"></param>
  /// <param name="before"></param>
  /// <param name="after"></param>
  /// <returns></returns>
  public static TokenListParser<TKind, T> Between<TKind, T, U, V>(
    this TokenListParser<TKind, T> self,
    TokenListParser<TKind, U> before,
    TokenListParser<TKind, V> after
  )
  {
    return before.IgnoreThen(self).ThenIgnore(after);
  }

  /// <summary>
  /// Parse an element between other.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="U"></typeparam>
  /// <param name="self"></param>
  /// <param name="other"></param>
  /// <returns></returns>
  public static TextParser<T> Between<T, U>(this TextParser<T> self, TextParser<U> other)
  {
    return self.Between(other, other);
  }

  /// <summary>
  /// Parse an element between other.
  /// </summary>
  /// <typeparam name="TKind"></typeparam>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="U"></typeparam>
  /// <typeparam name="V"></typeparam>
  /// <param name="self"></param>
  /// <param name="other"></param>
  /// <returns></returns>
  public static TokenListParser<TKind, T> Between<TKind, T, U, V>(
    this TokenListParser<TKind, T> self,
    TokenListParser<TKind, U> other
  )
  {
    return self.Between(other, other);
  }

  /// <summary>
  /// Parse any string matching the name of the provided enum.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public static TextParser<T> EnumParser<T>()
    where T : struct, Enum
  {
    return Parse.OneOf(
      Enum.GetValues<T>()
        .Select(value => Span.EqualTo(value.ToString()).Value(value).Try())
        .ToArray()
    );
  }

  /// <summary>
  /// Return the result if it matches any of the provided source strings.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static TextParser<string> StringIn(params string[] source)
  {
    return Parse.OneOf(source.Select(source => Span.EqualTo(source).Try().AsString()).ToArray());
  }

  /// <summary>
  /// Convert the character parser to a string.
  /// </summary>
  /// <param name="charParser"></param>
  /// <param name="atLeastOnce"></param>
  /// <returns></returns>
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

  /// <summary>
  /// Parse multiple elements up to the provided value.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="source"></param>
  /// <param name="value"></param>
  /// <returns></returns>
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
