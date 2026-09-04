namespace RonCS;

internal static class SetExtensions
{
  /// <summary>
  /// Try to find the element in the set.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="set"></param>
  /// <param name="predicate"></param>
  /// <param name="result"></param>
  /// <returns></returns>
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

  /// <summary>
  /// Try and find the element in the dictionary by key.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="V"></typeparam>
  /// <param name="dict"></param>
  /// <param name="keyPredicate"></param>
  /// <param name="result"></param>
  /// <returns></returns>
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
