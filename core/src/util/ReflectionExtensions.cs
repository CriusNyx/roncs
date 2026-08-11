using System.Reflection;
using CriusNyx.Util;

public static class ReflectionExtensions
{
  public static T Match<T>(this IEnumerable<T> options, Type?[] types)
    where T : MethodBase
  {
    return options.First(x => IsMatch(x, types));
  }

  private static bool IsMatch(MethodBase method, Type?[] types)
  {
    var parameters = method.GetParameters();
    if (parameters.Length < types.Length)
    {
      return false;
    }
    foreach (var (param, type) in parameters.OuterZip(types))
    {
      if (type == null)
      {
        continue;
      }
      if (type.IsAssignableTo(param.NotNull().GetType()))
      {
        continue;
      }
      return false;
    }
    return true;
  }

  /// <summary>
  /// Attempt to find a constructor and invoke it..
  /// </summary>
  /// <param name="t"></param>
  /// <param name="parameters"></param>
  /// <returns></returns>
  public static object? Construct(this Type t, params object[] parameters)
  {
    return t.GetConstructor(parameters.Select(x => x.GetType()).ToArray())?.Invoke(parameters);
  }
}
