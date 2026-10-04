using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using CriusNyx.Util;

public static class MemberSignature
{
  public static string? GetMemberSlug(MemberInfo memberInfo)
  {
    if (memberInfo is Type type)
    {
      return GetMemberSignature(type);
    }
    return $"{GetMemberSignature(memberInfo.DeclaringType)}:{GetMemberSignature(memberInfo)}";
  }

  public static string? GetMemberSignature(MemberInfo member)
  {
    return member switch
    {
      Type type => GetTypeSignature(type),
      FieldInfo field => GetFieldSignature(field),
      PropertyInfo property => GetPropertySignature(property),
      ConstructorInfo constructor => GetConstructorSignature(constructor),
      MethodInfo method => GetMethodSignature(method),
      _ => throw new NotImplementedException(),
    };
  }

  public static string GetTypeSignature(Type type)
  {
    var staticPart = (type.IsAbstract && type.IsSealed).And("static ");
    string declarationPart =
      type.IsClass.And("class ")
      ?? type.IsEnum.And("enum ")
      ?? type.IsInterface.And("interface ")
      ?? "";

    string? inherits = (type.BaseType != typeof(object) && !type.IsEnum)
      .And(type.BaseType?.Name)
      ?.Transform(x => $" : {x}");

    return $"{staticPart}{declarationPart}{type.Name}{inherits}";
  }

  public static string GetFieldSignature(FieldInfo field)
  {
    var staticPart = field.IsStatic.And("static ");
    var typeName = GetTypeName(field.FieldType);
    var name = field.Name;
    return $"{staticPart}{typeName} {name}";
  }

  public static string GetPropertySignature(PropertyInfo property)
  {
    var staticPart = property.GetAccessors(nonPublic: true).Any(x => x.IsStatic).And("static ");
    var typeName = GetTypeName(property.PropertyType);
    var propertyName = property.Name;
    var getterString = property.CanRead.And("get; ");
    var setterString = property.CanWrite.And("set; ");
    return $"{staticPart}{typeName} {propertyName} {{ {getterString}{setterString}}}";
  }

  public static string GetConstructorSignature(ConstructorInfo constructorInfo)
  {
    var typeName = constructorInfo
      .DeclaringType.NotNull(nameof(constructorInfo.DeclaringType))
      .Name;
    var args = GetParametersString(constructorInfo.GetParameters());
    return $"{typeName}{args}";
  }

  public static string GetMethodSignature(MethodInfo method)
  {
    var staticPart = method.IsStatic.And("static ");
    var returnType = GetTypeName(method.ReturnType);
    var genericParams = GetGenericParametersString(method.GetGenericArguments());
    var args = GetParametersString(method.GetParameters());
    return $"{staticPart}{returnType} {method.Name}{genericParams}{args}";
  }

  static string GetGenericParametersString(Type[] type)
  {
    if (type.Length == 0)
    {
      return "";
    }
    else
    {
      return $"<{type.Select(GetGenericParameterString).StringJoin(", ")}>";
    }
  }

  static string GetGenericParameterString(Type type)
  {
    return GetTypeName(type);
  }

  static string GetParametersString(ParameterInfo[] parameters)
  {
    var extensionPart = (
      parameters.FirstOrDefault()?.Member.GetCustomAttribute<ExtensionAttribute>() != null
    ).And("this ");

    return $"({extensionPart}{parameters.Select(GetParameterString).StringJoin(", ")})";
  }

  static string GetParameterString(ParameterInfo parameter)
  {
    var refModifier =
      parameter.IsOut.And("out ")
      ?? parameter.IsIn.And("in ")
      ?? parameter.ParameterType.IsByRef.And("ref ");

    var paramsPart = (parameter.GetCustomAttribute<ParamArrayAttribute>() != null).And("params ");

    var parameterType = GetTypeName(parameter.ParameterType);

    string nullability = "";
    if (parameter.ParameterType.IsClass)
    {
      if (new NullabilityInfoContext().Create(parameter).ReadState == NullabilityState.Nullable)
      {
        nullability = "?";
      }
    }

    var optionalPart = parameter.IsOptional.And($" = {parameter.DefaultValue.ToSource()}");

    return $"{paramsPart}{refModifier}{parameterType}{nullability} {parameter.Name}{optionalPart}";
  }

  private static string FriendlyName(string source)
  {
    return Regex.Replace(source, "`.*", "").Replace("&", "");
  }

  private static string GetTypeName(Type type)
  {
    if (type == typeof(void))
    {
      return "void";
    }
    if (type == typeof(bool))
    {
      return "bool";
    }
    if (type == typeof(byte))
    {
      return "byte";
    }
    if (type == typeof(sbyte))
    {
      return "sbyte";
    }
    if (type == typeof(char))
    {
      return "char";
    }
    if (type == typeof(decimal))
    {
      return "decimal";
    }
    if (type == typeof(double))
    {
      return "double";
    }
    if (type == typeof(float))
    {
      return "float";
    }
    if (type == typeof(int))
    {
      return "int";
    }
    if (type == typeof(int))
    {
      return "int";
    }
    if (type == typeof(uint))
    {
      return "uint";
    }
    if (type == typeof(nint))
    {
      return "nint";
    }
    if (type == typeof(nuint))
    {
      return "nuint";
    }
    if (type == typeof(long))
    {
      return "long";
    }
    if (type == typeof(ulong))
    {
      return "ulong";
    }
    if (type == typeof(short))
    {
      return "short";
    }
    if (type == typeof(ushort))
    {
      return "ushort";
    }
    if (type == typeof(string))
    {
      return "string";
    }
    if (type == typeof(object))
    {
      return "object";
    }

    if (type.AsGeneral() == typeof(Nullable<>))
    {
      var nullableType = type.GetGenericArguments().First().NotNull("genericArgument");
      return $"{GetTypeName(nullableType)}?";
    }

    var typeName = FriendlyName(type.Name);
    var genericArgs = GetGenericParametersString(type.GetGenericArguments());
    return $"{typeName}{genericArgs}";
  }
}
