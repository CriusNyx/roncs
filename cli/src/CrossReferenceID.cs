using System.Reflection;
using System.Text.RegularExpressions;
using CriusNyx.Util;
using RonCS;

public static class CrossReferenceID
{
  public static string GetIDForMember(MemberInfo member)
  {
    if (member is Type type)
    {
      return GetIDForType(type);
    }
    if (member is FieldInfo fieldInfo)
    {
      return GetIDForMember(fieldInfo.DeclaringType.NotNull(nameof(fieldInfo.DeclaringType)))
        + "."
        + fieldInfo.Name;
    }
    if (member is PropertyInfo propertyInfo)
    {
      return GetIDForMember(propertyInfo.DeclaringType.NotNull(nameof(propertyInfo.DeclaringType)))
        + "."
        + propertyInfo.Name;
    }
    if (member is ConstructorInfo constructor)
    {
      return GetIDForConstructor(constructor);
    }
    if (member is MethodInfo method)
    {
      return GetIDForMethod(method);
    }

    return member.ToString() ?? "";
  }

  static string GetIDForParameters(IEnumerable<ParameterInfo> members)
  {
    return members.Select(x => GetIDForMember(x.ParameterType)).StringJoin(",");
  }

  static string GetIDForConstructor(ConstructorInfo constructor)
  {
    var typePart = GetIDForMember(
      constructor.DeclaringType.NotNull(nameof(constructor.DeclaringType))
    );
    var parameterPart = GetIDForParameters(constructor.GetParameters());
    return $"{typePart}({parameterPart})";
  }

  static string GetIDForMethod(MethodInfo method)
  {
    var typePart = GetIDForMember(method.DeclaringType.NotNull(nameof(method.DeclaringType)));
    var genericParameterPart = method.IsGenericMethod
      ? GenericParams(method.GetGenericArguments())
      : "";
    var parameterPart = GetIDForParameters(method.GetParameters());
    return $"{typePart}.{method.Name}{genericParameterPart}({parameterPart})";
  }

  static string GetIDForType(Type type)
  {
    var namePart = Regex.Replace(type.Name, "`.*", "");
    var genericArgs = type.GetGenericArguments();

    if (type.IsGenericParameter)
    {
      return namePart;
    }

    var parentPart = (type.DeclaringType?.Transform(GetIDForMember) ?? type.Namespace)?.Transform(
      x => x + "."
    );

    var genericParameterPart = GetGenericParamsString(type);

    return $"{parentPart}{namePart}{genericParameterPart}";
  }

  static string GetGenericParamsString(Type type)
  {
    var name = type.FullName;
    if (type.GenericTypeArguments.Length > 0)
    {
      return type.GenericTypeArguments.Transform(GenericParams);
    }
    var genericArgument = type.GetGenericArguments();
    if (genericArgument.Length > 0)
    {
      return GenericParams(genericArgument);
    }
    // if (type.IsGenericType && genericParams?.Length > 0)
    // {
    //   return GenericParams(genericParams);
    // }
    return "";
  }

  static string GenericParams(IEnumerable<MemberInfo> members)
  {
    return $"<{members.Select(GetIDForMember).StringJoin(",")}>";
  }

  public static T[]? NullIfEmpty<T>(this T[]? array)
  {
    if (array?.Length > 0)
    {
      return array;
    }
    return null;
  }

  public static U? Try<T, U>(this T source, Func<T, U?> transformer)
  {
    try
    {
      return transformer(source);
    }
    catch
    {
      return default;
    }
  }
}
