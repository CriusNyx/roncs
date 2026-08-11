using System.Reflection;
using CriusNyx.Util;

public class TypeConversionCache
{
  /// <summary>
  /// Store known type converters.
  /// </summary>
  private static Dictionary<(Type, Type), Func<object, object>> cache =
    new Dictionary<(Type, Type), Func<object, object>>();

  public static object ConvertTo(object source, Type target)
  {
    // It might be intended that it converts to null.
    return GetConverter(source.GetType(), target)?.Invoke(source)!;
  }

  public static T ConvertTo<T>(object source)
  {
    // It might be intended that it converts to null.
    return (T)ConvertTo(source, typeof(T));
  }

  /// <summary>
  /// Get type converter for type.
  /// </summary>
  /// <param name="fromType"></param>
  /// <param name="intoType"></param>
  /// <returns></returns>
  public static Func<object, object> GetConverter(Type fromType, Type intoType)
  {
    return cache.GetOrSet((fromType, intoType), () => _GetConverter(fromType, intoType));
  }

  /// <summary>
  /// Create memoized type converter.
  /// </summary>
  /// <param name="fromType"></param>
  /// <param name="intoType"></param>
  /// <returns></returns>
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

public static class TypeConversionExtensions
{
  public static T RonConvert<T>(this object source)
  {
    return TypeConversionCache.ConvertTo<T>(source);
  }

  public static object RonConvert(this object source, Type t)
  {
    return TypeConversionCache.ConvertTo(source, t);
  }
}
