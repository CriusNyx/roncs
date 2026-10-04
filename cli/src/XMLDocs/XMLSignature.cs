using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CriusNyx.Util;
using NuDoq;

public static class XMLSignature
{
  public static XNode GetSearchSignature(
    MemberInfo member,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    return new XElement("signature", GetSearchSignatureNodes(member, membersToIds));
  }

  private static IEnumerable<XNode> GetSearchSignatureNodes(
    MemberInfo member,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    IEnumerable<XNode> Parameters(IEnumerable<ParameterInfo> parameters)
    {
      return
      [
        Text("("),
        .. parameters
          .Select(x => new XNode[] { Var(x.Name.NotNull()) })
          .IntersperseWith(() => [Text(","), Space()])
          .Flatten(),
        Text(")"),
      ];
    }

    IEnumerable<XNode> GenericParameters(IEnumerable<Type> parameters)
    {
      if (parameters.Count() == 0)
      {
        return [];
      }
      return
      [
        Text("<"),
        .. parameters
          .Select(x => new XNode[] { Type(x.Name.NotNull()) })
          .IntersperseWith(() => [Text(","), Space()])
          .Flatten(),
        Text(">"),
      ];
    }

    if (member is Type type)
    {
      return [Type(type.Name), .. GetGenericParameters(type.GenericTypeArguments, membersToIds)];
    }
    if (member is ConstructorInfo constructor)
    {
      return
      [
        .. GetSearchSignatureNodes(constructor.DeclaringType.NotNull(), membersToIds),
        .. Parameters(constructor.GetParameters()),
      ];
    }
    if (member is MethodInfo method)
    {
      return
      [
        .. GetSearchSignatureNodes(method.DeclaringType.NotNull(), membersToIds),
        Text("."),
        Method(method.Name),
        .. GenericParameters(method.GetGenericArguments()),
        .. Parameters(method.GetParameters()),
      ];
    }
    if (member is FieldInfo field)
    {
      return
      [
        .. GetSearchSignatureNodes(field.DeclaringType.NotNull(), membersToIds),
        Text("."),
        Var(field.Name),
      ];
    }
    if (member is PropertyInfo property)
    {
      return
      [
        .. GetSearchSignatureNodes(property.DeclaringType.NotNull(), membersToIds),
        Text("."),
        Var(property.Name),
      ];
    }
    throw new NotImplementedException();
  }

  public static IEnumerable<XNode> GetMemberSignature(
    MemberInfo member,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    string signatureType = member switch
    {
      Type _ => "type",
      FieldInfo => "field",
      PropertyInfo => "property",
      ConstructorInfo => "constructor",
      MethodInfo => "method",
      _ => throw new NotImplementedException(),
    };

    return (
      member switch
      {
        Type type => GetTypeSignature(type, membersToIds),
        FieldInfo field => GetFieldSignature(field, membersToIds),
        PropertyInfo property => GetPropertySignature(property, membersToIds),
        ConstructorInfo constructor => GetConstructorSignature(constructor, membersToIds),
        MethodInfo method => GetMethodSignature(method, membersToIds),
        _ => throw new NotImplementedException(),
      }
    ).Transform(x =>
      new XNode[] { new XElement("signature", x, new XAttribute("type", signatureType)) }
    );
  }

  public static IEnumerable<XNode> GetTypeSignature(
    Type type,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    var staticPart = (type.IsAbstract && type.IsSealed).And(Keyword("static"));
    XNode? declarationPart =
      type.IsClass.And(Keyword("class"))
      ?? type.IsEnum.And(Keyword("enum"))
      ?? type.IsInterface.And(Keyword("interface"));

    var inherits =
      (type.BaseType != typeof(object) && !type.IsEnum)
        .And(type.BaseType?.Transform((x) => GetTypeName(x, membersToIds)))
        ?.Transform(x => Defined([Space(), Text(":"), Space(), .. x]))
      ?? [];

    return Defined([
      .. staticPart?.Transform(x => new XNode[] { x, Space() }) ?? [],
      declarationPart,
      Space(),
      .. GetTypeName(type, membersToIds),
      .. inherits,
    ]);
  }

  public static IEnumerable<XNode> GetFieldSignature(
    FieldInfo field,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    var staticPart = field.IsStatic.And(new XNode[] { Keyword("static"), Space() }) ?? [];
    var typeName = GetTypeName(field.FieldType, membersToIds);
    var name = Var(field.Name);
    return Defined([.. staticPart, .. typeName, Space(), name]);
  }

  public static IEnumerable<XNode> GetPropertySignature(
    PropertyInfo property,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    var staticPart =
      property
        .GetAccessors(nonPublic: true)
        .Any(x => x.IsStatic)
        .And(new XNode[] { Keyword("static"), Space() })
      ?? [];
    var typeName = GetTypeName(property.PropertyType, membersToIds);
    var propertyName = Var(property.Name);

    var getterString =
      property.CanRead.And<IEnumerable<XNode>>([Keyword("get"), Text(";"), Space()]) ?? [];
    var setterString =
      property.CanWrite.And<IEnumerable<XNode>>([Keyword("get"), Text(";"), Space()]) ?? [];

    return Defined([
      .. staticPart,
      .. typeName,
      Space(),
      propertyName,
      Space(),
      Text("{"),
      Space(),
      .. getterString,
      .. setterString,
      Text("}"),
    ]);
  }

  public static IEnumerable<XNode> GetConstructorSignature(
    ConstructorInfo constructorInfo,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    var typeName = GetTypeName(constructorInfo.DeclaringType.NotNull(), membersToIds);

    var args = GetParameters(constructorInfo.GetParameters(), membersToIds);

    return Defined([.. typeName, .. args]);
  }

  public static IEnumerable<XNode> GetMethodSignature(
    MethodInfo method,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    var staticPart = method.IsStatic.And(new XNode[] { Keyword("static"), Space() }) ?? [];
    XNode[] returnType = [.. GetTypeName(method.ReturnType, membersToIds), Space()];
    var methodName = Method(method.Name);
    var genericParams = GetGenericParameters(method.GetGenericArguments(), membersToIds);
    var args = GetParameters(method.GetParameters(), membersToIds);

    return Defined([.. staticPart, .. returnType, methodName, .. genericParams, .. args]);
  }

  static IEnumerable<XNode> GetGenericParameters(
    Type[] type,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    if (type.Length == 0)
    {
      return [];
    }
    else
    {
      return
      [
        Text("<"),
        .. type.Select((x) => GetGenericParameter(x, membersToIds))
          .IntersperseWith(() => [Space()])
          .Flatten(),
        Text(">"),
      ];
    }
  }

  static IEnumerable<XNode> GetGenericParameter(
    Type type,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    return GetTypeName(type, membersToIds);
  }

  static IEnumerable<XNode> GetParameters(
    ParameterInfo[] parameters,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    if (parameters.Length == 0)
    {
      return [Text("()")];
    }

    var extensionPart = (
      parameters.FirstOrDefault()?.Member.GetCustomAttribute<ExtensionAttribute>() != null
    ).And(Keyword("this"));

    return Defined([
      Space(),
      Text("("),
      Break(),
      extensionPart,
      .. parameters
        .Select((x) => GetParameterNodes(x, membersToIds))
        .Select(x => Tab().ThenConcat(x))
        .IntersperseWith(() => [Text(","), Break()])
        .Flatten(),
      Break(),
      Text(")"),
    ]);
  }

  static IEnumerable<XNode> GetParameterNodes(
    ParameterInfo parameter,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    var refModifier =
      (
        parameter.IsOut.And(Keyword("out"))
        ?? parameter.IsIn.And(Keyword("in"))
        ?? parameter.ParameterType.IsByRef.And(Keyword("ref"))
      )?.Transform(x => new XNode[] { x, Space() })
      ?? [];

    var paramsPart =
      (parameter.GetCustomAttribute<ParamArrayAttribute>() != null).And(
        new XNode[] { Keyword("params"), Space() }
      ) ?? [];

    var parameterType = GetTypeName(parameter.ParameterType, membersToIds);

    XNode? nullability = null;
    if (parameter.ParameterType.IsClass)
    {
      if (new NullabilityInfoContext().Create(parameter).ReadState == NullabilityState.Nullable)
      {
        nullability = Text("?");
      }
    }

    var optionalPart =
      parameter.IsOptional.And<IEnumerable<XNode>>([
        Space(),
        Text("="),
        Space(),
        .. ToSourceXml(parameter.DefaultValue, membersToIds),
      ]) ?? [];

    return Defined([
      .. paramsPart,
      .. refModifier,
      .. parameterType,
      nullability,
      Space(),
      Var(parameter.Name.NotNull()),
      .. optionalPart,
    ]);
  }

  private static XNode FriendlyName(string source)
  {
    return Text(Regex.Replace(source, "`.*", "").Replace("&", ""));
  }

  private static IEnumerable<XNode> GetTypeName(
    Type type,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    if (type == typeof(void))
    {
      return [Keyword("void")];
    }
    if (type == typeof(bool))
    {
      return [Keyword("bool")];
    }
    if (type == typeof(byte))
    {
      return [Keyword("byte")];
    }
    if (type == typeof(sbyte))
    {
      return [Keyword("sbyte")];
    }
    if (type == typeof(char))
    {
      return [Keyword("char")];
    }
    if (type == typeof(decimal))
    {
      return [Keyword("decimal")];
    }
    if (type == typeof(double))
    {
      return [Keyword("double")];
    }
    if (type == typeof(float))
    {
      return [Keyword("float")];
    }
    if (type == typeof(int))
    {
      return [Keyword("int")];
    }
    if (type == typeof(int))
    {
      return [Keyword("int")];
    }
    if (type == typeof(uint))
    {
      return [Keyword("uint")];
    }
    if (type == typeof(nint))
    {
      return [Keyword("nint")];
    }
    if (type == typeof(nuint))
    {
      return [Keyword("nuint")];
    }
    if (type == typeof(long))
    {
      return [Keyword("long")];
    }
    if (type == typeof(ulong))
    {
      return [Keyword("ulong")];
    }
    if (type == typeof(short))
    {
      return [Keyword("short")];
    }
    if (type == typeof(ushort))
    {
      return [Keyword("ushort")];
    }
    if (type == typeof(string))
    {
      return [Keyword("string")];
    }
    if (type == typeof(object))
    {
      return [Keyword("object")];
    }

    if (type.AsGeneral() == typeof(Nullable<>))
    {
      var nullableType = type.GetGenericArguments().First().NotNull("genericArgument");
      return GetTypeName(nullableType, membersToIds).Concat([Text("?")]);
    }
    if (type.IsArray)
    {
      var rank = type.GetArrayRank();
      var elementType = type.GetElementType().NotNull();
      var text = Text($"[{Enumerable.Range(0, rank - 1).Select(x => ",").StringJoin()}]");
      return [.. GetTypeName(elementType, membersToIds), text];
    }

    var typeName = Type(FriendlyName(type.Name), type, membersToIds);
    var genericArgs = GetGenericParameters(
      NotEmpty(type.GetGenericArguments(), type.GetGenericArguments()),
      membersToIds
    );
    return typeName.ThenConcat(genericArgs);
  }

  private static T[] NotEmpty<T>(params T[][] arrays)
  {
    foreach (var array in arrays)
    {
      if (array.Length > 0)
      {
        return array;
      }
    }
    return [];
  }

  private static XNode Keyword(string value)
  {
    return new XElement("keyword", value);
  }

  private static XNode Type(string name)
  {
    return new XElement("type", name);
  }

  private static XNode Var(string name)
  {
    return new XElement("variable", name);
  }

  private static XNode Type(
    XNode node,
    Type? reference,
    IDictionary<MemberInfo, string> memberDictionary
  )
  {
    var output = new XElement("type", node);
    if (reference is Type && ElementURL.GetElementUrl(reference, memberDictionary) is string refUrl)
    {
      output.Add(new XAttribute("refUrl", refUrl));
    }
    return output;
  }

  private static XNode Method(string value)
  {
    return new XElement("method", value);
  }

  private static XNode Text(string value)
  {
    return new XText(value);
  }

  private static XNode String(string value)
  {
    return new XElement("string", value);
  }

  private static XNode Char(string value)
  {
    return new XElement("char", value);
  }

  private static XNode Num(string value)
  {
    return new XElement("num", value);
  }

  private static XNode Space()
  {
    return new XElement("space");
  }

  private static XNode Tab()
  {
    return new XElement("tab");
  }

  private static XNode Break()
  {
    return new XElement("break");
  }

  private static IEnumerable<XNode> Defined(params XNode?[] nodes)
  {
    return nodes.WhereAs<XNode>();
  }

  private static IEnumerable<XNode> ToSourceXml(
    object? o,
    IDictionary<MemberInfo, string> membersToIds
  )
  {
    if (o == null)
    {
      return [Keyword("null")];
    }
    if (o.GetType().IsEnum)
    {
      return
      [
        Type(FriendlyName(o.GetType().Name), o.GetType(), membersToIds),
        Text("."),
        Var(o.ToString().NotNull()),
      ];
    }
    return o switch
    {
      string s => [String($"\"{o}\"")],
      char c => [Char($"'{o}'")],
      bool b => b ? [Keyword("true")] : [Keyword("false")],
      byte b => [Num(b.ToString())],
      sbyte b => [Num(b.ToString())],
      short s => [Num(s.ToString())],
      ushort s => [Num(s.ToString())],
      int i => [Num(i.ToString())],
      uint i => [Num(i.ToString())],
      long l => [Num(l.ToString())],
      ulong l => [Num(l.ToString())],
      float f => [Num(f.ToString())],
      double d => [Num(d.ToString())],
      decimal m => [Num(m.ToString())],
      _ => [Text(o.ToString() ?? "")],
    };
  }
}
