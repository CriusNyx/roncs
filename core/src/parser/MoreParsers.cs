using CriusNyx.Util;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

public static class MoreParsers
{
  public static TextParser<string> AsString(this TextParser<char> source)
  {
    return source.AtLeastOnce().Select(x => new string(x.ToArray()));
  }

  public static TextParser<string> AsString(this TextParser<char[]> source)
  {
    return source.Select(x => new string(x));
  }

  public static TextParser<string> AsString(this TextParser<IEnumerable<char>> source)
  {
    return source.Select(x => new string(x.ToArray()));
  }

  public static TextParser<string> AsString(this TextParser<string> source)
  {
    return source.AtLeastOnce().Select(x => x.StringJoin());
  }

  public static TextParser<string> AsString(this TextParser<TextSpan> source)
  {
    return source
      .AtLeastOnce()
      .Select(spans => spans.Select(span => span.ToStringValue()).StringJoin());
  }

  public static TextParser<IEnumerable<T>> AtLeastOnce<T>(this TextParser<T> source)
  {
    return from leading in source from rest in source.Many() select leading.ThenConcat(rest);
  }

  public static TextParser<IEnumerable<T>> UpTo<T>(this TextParser<T> source, int value)
  {
    if (value < 1)
    {
      throw new NotImplementedException();
    }
    else if (value == 1)
    {
      return source.Select(x => new T[] { x } as IEnumerable<T>);
    }
    else
    {
      return from first in source
        from rest in UpTo(source, value - 1).OptionalOrDefault([])
        select first.ThenConcat(rest);
    }
  }

  public static TextParser<Position> Position = Span.MatchedBy(Span.Length(0))
    .Select(x => x.Position)
    // Never consume input.
    .Try();

  public static TextParser<string> Until<T>(params TextParser<T>[] parsers)
  {
    return Parse.Not(Parse.OneOf(parsers)).IgnoreThen(Character.AnyChar).AsString();
  }

  public static TextParser<T> ThenIgnore<T, U>(this TextParser<T> self, TextParser<U> other)
  {
    return from a in self from b in other select a;
  }

  public static TextParser<T> Between<T, U>(this TextParser<T> self, TextParser<U> outside)
  {
    return self.Between(outside, outside);
  }

  public static TextParser<T> EnumParser<T>()
    where T : struct, Enum
  {
    var values = Enum.GetValues<T>();
    return Parse.OneOf(
      values.Select(value => Span.EqualTo(value.ToString()).Try().Value(value)).ToArray()
    );
  }

  public static TextParser<Struct?> OrNull<Struct>(this TextParser<Struct> source)
    where Struct : struct
  {
    return source.Select(x => (Struct?)x).OptionalOrDefault();
  }

  public static TextParser<object> Ignore<T>(this TextParser<T> source)
  {
    return source.Value(null as object)!;
  }

  public static TextParser<string> Concat(
    this TextParser<char> original,
    TextParser<char> next = null!
  )
  {
    if (next is not null)
    {
      return original.Then(first => next.Many().Select((rest) => new string([first, .. rest])));
    }
    else
    {
      return original.Many().AsString();
    }
  }
}
