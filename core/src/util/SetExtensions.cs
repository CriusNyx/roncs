using Microsoft.VisualBasic;

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
}
