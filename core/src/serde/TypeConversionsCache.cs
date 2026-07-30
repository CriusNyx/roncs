using System.Reflection;
using CriusNyx.Util;

public class TypeConversionCache
{
  private static Dictionary<(Type, Type), Func<object, object>> cache =
    new Dictionary<(Type, Type), Func<object, object>>();

  public static Func<object, object> GetConverter(Type fromType, Type intoType)
  {
    return cache.GetOrSet((fromType, intoType), () => _GetConverter(fromType, intoType));
  }

  private static Func<object, object> _GetConverter(Type fromType, Type intoType)
  {
    if (
      intoType
        .GetMethods()
        .FirstOrDefault(method =>
          method.IsStatic
          && method.GetCustomAttribute<RonFromAttribute>() is RonFromAttribute
          && method.ReturnType.IsAssignableTo(intoType)
          && method.GetParameters().Match(param => fromType.IsAssignableTo(param.ParameterType))
        )
      is MethodInfo from
    )
    {
      return (source) => from.Invoke(null, [source])!;
    }

    var methods = fromType.GetMethods();

    if (
      fromType
        .GetMethods()
        .FirstOrDefault(method =>
          !method.IsStatic
          && method.GetCustomAttribute<RonIntoAttribute>() is RonIntoAttribute
          && method.ReturnType.IsAssignableTo(intoType)
          && method.GetParameters().Length == 0
        )
      is MethodInfo into
    )
    {
      return (source) => into.Invoke(source, [])!;
    }
    return (source) => source;
  }
}
