public static class SetExtensions
{
  public static bool TryFind<T>(this ISet<T> set, Func<T, bool> predicate, out T result)
  {
    foreach (var element in set)
    {
      if (predicate(element))
      {
        result = element;
        return true;
      }
    }
    result = default!;
    return false;
  }

  public static bool TryFindByKey<T, V>(
    this IDictionary<T, V> dict,
    Func<T, bool> keyPredicate,
    out V result
  )
  {
    foreach (var (key, value) in dict)
    {
      if (keyPredicate(key))
      {
        result = value;
        return true;
      }
    }
    result = default!;
    return false;
  }
}
