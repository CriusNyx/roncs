using System.Reflection;
using CriusNyx.Util;

public static class ReflectionExtensions
{
  public static T? Match<T>(this IEnumerable<T> options, Type?[] types)
    where T : MethodBase
  {
    return options.FirstOrDefault(x => IsMatch(x, types));
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
      if (type.IsAssignableTo(param.NotNull().ParameterType))
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

  public static object? GetMemberValue(this MemberInfo member, object source)
  {
    if (member is FieldInfo field)
    {
      return field.GetValue(source);
    }
    if (member is PropertyInfo property)
    {
      return property.GetValue(source);
    }
    throw new InvalidOperationException();
  }

  public static IEnumerable<MemberInfo> GetFieldsAndProperties(
    this Type type,
    BindingFlags flags = BindingFlags.Default
  )
  {
    return type.GetMembers(flags).Where(x => x is FieldInfo || x is PropertyInfo);
  }

  public static bool IsStaticMember(this MemberInfo member)
  {
    if (member is FieldInfo field)
    {
      return field.IsStatic;
    }
    if (member is PropertyInfo property)
    {
      return property.GetGetMethod()?.IsStatic ?? false;
    }
    throw new InvalidOperationException();
  }

  public static bool IsPublicMember(this MemberInfo member)
  {
    if (member is FieldInfo field)
    {
      return field.IsPublic;
    }
    if (member is PropertyInfo property)
    {
      return property.CanRead;
    }
    throw new InvalidOperationException();
  }

  public static Type MemberValueType(this MemberInfo member)
  {
    if (member is FieldInfo field)
    {
      return field.FieldType;
    }
    if (member is PropertyInfo property)
    {
      return property.PropertyType;
    }
    throw new InvalidOperationException();
  }

  public static MemberInfo? GetFieldOrProperty(
    this Type type,
    string memberName,
    BindingFlags flags = BindingFlags.Default
  )
  {
    return type.GetField(memberName, flags) as MemberInfo ?? type.GetProperty(memberName, flags);
  }

  public static MemberInfo? GetRonField(this Type type, string memberName)
  {
    if (type.GetField(memberName, (BindingFlags)(-1)) is FieldInfo field && field.IsRonMember())
    {
      return field;
    }
    if (
      type.GetProperty(memberName, (BindingFlags)(-1)) is PropertyInfo property
      && property.IsRonMember()
    )
    {
      return property;
    }
    return null;
  }

  public static void AssignMember(this MemberInfo member, object? source, object value)
  {
    if (member is FieldInfo field)
    {
      field.SetValue(source, value);
    }
    if (member is PropertyInfo property)
    {
      property.SetValue(source, value);
    }
  }

  public static bool IsRonMember(this MemberInfo member)
  {
    if (member.GetCustomAttribute<RonIncludeAttribute>() is RonIncludeAttribute)
    {
      return true;
    }
    if (member.GetCustomAttribute<RonExcludeAttribute>() is RonExcludeAttribute)
    {
      return false;
    }
    return !member.IsStaticMember() && member.IsPublicMember();
  }
}
