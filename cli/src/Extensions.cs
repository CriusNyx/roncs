using System.Collections;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using CriusNyx.Util;
using NuDoq;

public static class Extensions
{
  public static T? And<T>(this bool value, T other)
  {
    if (value)
    {
      return other;
    }
    return default;
  }

  public static T? And<T>(this bool value, Func<T> action)
  {
    if (value)
    {
      return action();
    }
    return default;
  }

  public static Type AsGeneral(this Type type)
  {
    return type.IsGenericType.And(type.GetGenericTypeDefinition) ?? type;
  }

  public static string ToSource(this object? o)
  {
    if (o == null)
    {
      return "null";
    }
    if (o.GetType().IsEnum)
    {
      return $"{o.GetType().Name}.{o}";
    }
    return o switch
    {
      string s => $"\"{s.Replace("\"", "\\\"")}\"",
      char c => $"'{c}'",
      bool b => b ? "true" : "false",
      byte b => $"{b}uy",
      sbyte b => $"{b}",
      short s => $"{s}",
      ushort s => $"{s}u",
      int i => $"{i}",
      uint i => $"{i}u",
      long l => $"{l}L",
      ulong l => $"{l}UL",
      float f => $"{f}f",
      double d => $"{d}d",
      decimal m => $"{m}m",
      _ => o.ToString() ?? "",
    };
  }

  public static IEnumerable<T> IntersperseWith<T>(this IEnumerable<T> source, Func<T> createOther)
  {
    var first = true;
    foreach (var element in source)
    {
      if (!first)
      {
        yield return createOther();
      }
      first = false;
      yield return element;
    }
  }

  public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> values)
  {
    return values.SelectMany(x => x);
  }

  public static PropertyInfo? GetBaseOrInterfaceProperty(this PropertyInfo property)
  {
    // Check getter.
    if (property.GetGetMethod() is MethodInfo getter)
    {
      if (getter.GetBaseOrInterfaceMethod() is MethodInfo baseMethod)
      {
        if (
          baseMethod
            .DeclaringType?.GetProperties()
            .FirstOrDefault(x => x.GetGetMethod() == baseMethod)
          is PropertyInfo baseProperty
        )
        {
          return baseProperty;
        }
      }
    }
    // Check setter.
    if (property.GetSetMethod() is MethodInfo setter)
    {
      if (setter.GetBaseOrInterfaceMethod() is MethodInfo baseMethod)
      {
        if (
          baseMethod
            .DeclaringType?.GetProperties()
            .FirstOrDefault(x => x.GetSetMethod() == baseMethod)
          is PropertyInfo baseProperty
        )
        {
          return baseProperty;
        }
      }
    }
    return null;
  }

  public static MethodInfo? GetBaseOrInterfaceMethod(this MethodInfo methodInfo)
  {
    if (methodInfo.GetInterfaceMethodDeclaration() is MethodInfo interfaceMethod)
    {
      return interfaceMethod;
    }
    var baseMethod = methodInfo.GetBaseDefinition();
    if (baseMethod != methodInfo)
    {
      return baseMethod;
    }
    return null;
  }

  public static MethodInfo? GetInterfaceMethodDeclaration(this MethodInfo method)
  {
    var type = method.DeclaringType;

    foreach (var @interface in type!.GetInterfaces())
    {
      var map = type.GetInterfaceMap(@interface);

      for (int i = 0; i < map.TargetMethods.Length; i++)
      {
        if (map.TargetMethods[i] == method)
          return map.InterfaceMethods[i];
      }
    }

    return null;
  }

  public static string? GetAttribute(this XElement element, string name)
  {
    return element.Attributes().FirstOrDefault(x => x.Name == name)?.Value;
  }

  public static IEnumerable<XNode> XPathLocal(this XNode node, string xpath)
  {
    return node.CreateNavigator()
      .Select(xpath)
      .AsNotNull<IEnumerable>()
      .Cast<XPathNavigator>()
      .Select(x => x.UnderlyingObject.AsNotNull<XNode>());
  }

  public static string XMLValueText(this XNode node)
  {
    StringBuilder builder = new StringBuilder();
    XMLValueText(node, builder);
    return builder.ToString();
  }

  private static void XMLValueText(this XNode node, StringBuilder builder)
  {
    if (node is XText text)
    {
      builder.Append(text.Value);
    }
    else if (node is XElement element)
    {
      foreach (var child in element.Nodes())
      {
        XMLValueText(child, builder);
      }
    }
  }
}
