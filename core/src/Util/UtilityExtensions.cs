using System.Collections;

namespace RonCS;

internal static class UtilityExtensions
{
  private static NullReferenceException CreateNullException(string? valueName)
  {
    if (valueName == null)
    {
      return new NullReferenceException();
    }
    else
    {
      return new NullReferenceException(
        $"\"{valueName}\" was expected to not be null, but it was null."
      );
    }
  }

  internal static T NotNull<T>(this T? value, string? valueName = null)
  {
    if (value == null)
    {
      throw CreateNullException(valueName);
    }
    return value;
  }

  internal static T AsNotNull<T>(this object? value, string? valueName = null)
  {
    if (value is T t)
    {
      return t;
    }
    throw CreateNullException(valueName);
  }

  internal static string StringJoin(this IEnumerable<string> strings, string? separator = "")
  {
    return string.Join(separator, strings);
  }

  internal static string Indent(this string source, string indent)
  {
    return indent + source.Replace("\n", "\n" + indent);
  }

  internal static U Transform<T, U>(this T value, Func<T, U> transformer)
  {
    return transformer(value);
  }

  internal static IEnumerable<T> ThenConcat<T>(this T start, IEnumerable<T> rest)
  {
    return new T[] { start }.Concat(rest);
  }

  internal static IEnumerable<(T? left, U? right)> OuterZip<T, U>(
    this IEnumerable<T> source,
    IEnumerable<U> other
  )
  {
    var a = source.GetEnumerator();
    var b = other.GetEnumerator();
    while (a.MoveNext() && b.MoveNext())
    {
      yield return (a.Current, b.Current);
    }
    while (a.MoveNext())
    {
      yield return (a.Current, default);
    }
    while (b.MoveNext())
    {
      yield return (default, b.Current);
    }
  }

  internal static IEnumerable<(T value, int index)> WithIndex<T>(this IEnumerable<T> source)
  {
    int index = 0;
    foreach (var element in source)
    {
      yield return (element, index++);
    }
  }

  internal static T Touch<T>(this T value, Action<T> action)
  {
    action(value);
    return value;
  }

  internal static Value GetOrSet<Key, Value>(
    this IDictionary<Key, Value> dict,
    Key key,
    Func<Value> factory
  )
  {
    if (dict.TryGetValue(key, out var result))
    {
      return result;
    }
    return dict[key] = factory();
  }

  internal static T? As<T>(this object? o)
  {
    if (o is T t)
    {
      return t;
    }
    return default;
  }

  internal static bool Match<T>(this IEnumerable<T> source, params Func<T, bool>[] predicates)
  {
    var enumerator = source.GetEnumerator();
    foreach (var pred in predicates)
    {
      if (!enumerator.TryConsume(out var value))
      {
        return false;
      }
      if (!pred(value))
      {
        return false;
      }
    }
    return true;
  }

  internal static object Consume(this IEnumerator source)
  {
    if (!source.MoveNext())
    {
      throw new InvalidOperationException();
    }
    return source.Current;
  }

  internal static bool TryConsume<T>(this IEnumerator<T> source, out T value)
  {
    if (source.MoveNext())
    {
      value = source.Current;
      return true;
    }
    value = default!;
    return false;
  }

  internal static (T?, U?, V?, W?) Take<T, U, V, W>(this IEnumerable values)
  {
    var enumerator = values.GetEnumerator();
    return (
      enumerator.Consume().As<T>(),
      enumerator.Consume().As<U>(),
      enumerator.Consume().As<V>(),
      enumerator.Consume().As<W>()
    );
  }
}
