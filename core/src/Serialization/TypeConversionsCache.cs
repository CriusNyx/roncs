using System.Reflection;

namespace RonCS;

internal class TypeConversionCache
{
  /// <summary>
  /// Store known type converters.
  /// </summary>
  private static Dictionary<(Type, Type), Func<object, object>> cache =
    new Dictionary<(Type, Type), Func<object, object>>();

  /// <summary>
  /// Convert the object to the target type using any known conversion between the two.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="target"></param>
  /// <returns></returns>
  public static object ConvertTo(object source, Type target)
  {
    // It might be intended that it converts to null.
    return GetConverter(source.GetType(), target)?.Invoke(source)!;
  }

  /// <summary>
  /// Convert the element to the target type T.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="source"></param>
  /// <returns></returns>
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
    if (
      fromType
        .GetMethods()
        .FirstOrDefault(method =>
          method.IsStatic
          && method.GetCustomAttribute<RonIntoAttribute>() is RonIntoAttribute
          && method.ReturnType.IsAssignableTo(intoType)
          && method.GetParameters().Match(x => x.ParameterType == fromType)
        )
      is MethodInfo staticInto
    )
    {
      return (source) => staticInto.Invoke(null, [source])!;
    }
    return (source) => source;
  }
}

public static class TypeConversionExtensions
{
  /// <summary>
  /// Use the RonConvert algorithm to convert the element to the type T.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="source"></param>
  /// <returns></returns>
  public static T RonConvert<T>(this object source)
  {
    return TypeConversionCache.ConvertTo<T>(source);
  }

  /// <summary>
  /// Use the RonConvert algorithm to convert the element to the type T.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="targetType"></param>
  /// <returns></returns>
  public static object RonConvert(this object source, Type targetType)
  {
    return TypeConversionCache.ConvertTo(source, targetType);
  }
}
