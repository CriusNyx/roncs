using System.Reflection;

namespace RonCS;

public static class ReflectionExtensions
{
  /// <summary>
  /// Return the first element that matches the options provided.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="options"></param>
  /// <param name="types"></param>
  /// <returns></returns>
  public static T? MatchFirst<T>(this IEnumerable<T> options, Type?[] types)
    where T : MethodBase
  {
    return options.FirstOrDefault(x => IsMatch(x, types));
  }

  /// <summary>
  /// Return true if the types can be used to invoke the method.
  /// </summary>
  /// <param name="method"></param>
  /// <param name="types"></param>
  /// <returns></returns>
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

  /// <summary>
  /// Get the value of the member on the object if possible.
  /// </summary>
  /// <param name="member"></param>
  /// <param name="source"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
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

  /// <summary>
  /// Get a list of fields and properties on the object.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="flags"></param>
  /// <returns></returns>
  public static IEnumerable<MemberInfo> GetFieldsAndProperties(
    this Type type,
    BindingFlags flags = BindingFlags.Default
  )
  {
    return type.GetMembers(flags).Where(x => x is FieldInfo || x is PropertyInfo);
  }

  /// <summary>
  /// Returns tue if the element is a static member.
  /// </summary>
  /// <param name="member"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
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

  /// <summary>
  /// Returns true if the element is publicly visible.
  /// </summary>
  /// <param name="member"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
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

  /// <summary>
  /// Returns the type of the element value if it can be determined.
  /// </summary>
  /// <param name="member"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
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

  /// <summary>
  /// Gets a field or property by name.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="memberName"></param>
  /// <param name="flags"></param>
  /// <returns></returns>
  public static MemberInfo? GetFieldOrProperty(
    this Type type,
    string memberName,
    BindingFlags flags = BindingFlags.Default
  )
  {
    return type.GetField(memberName, flags) as MemberInfo ?? type.GetProperty(memberName, flags);
  }

  /// <summary>
  /// Get member with the provided member name or with an attribute matching the member name.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="memberName"></param>
  /// <returns></returns>
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

  /// <summary>
  /// Assign the member if possible. Otherwise throw an exception.
  /// </summary>
  /// <param name="member"></param>
  /// <param name="source"></param>
  /// <param name="value"></param>
  public static void AssignMember(this MemberInfo member, object? source, object value)
  {
    if (member is FieldInfo field)
    {
      field.SetValue(source, value);
    }
    else if (member is PropertyInfo property)
    {
      property.SetValue(source, value);
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Returns true if the member should be included in ron serialization.
  /// </summary>
  /// <param name="member"></param>
  /// <returns></returns>
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
