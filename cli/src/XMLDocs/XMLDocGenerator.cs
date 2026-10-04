using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using CriusNyx.Util;
using NuDoq;
using RonCS;

public static class XMLDocGenerator
{
  public static void GenerateXMLDocsFile(string outFile)
  {
    var xml = GenerateXMLDocs();
    File.WriteAllText(outFile, xml);
  }

  public static string GenerateXMLDocs()
  {
    var assembly = DocReader.Read(typeof(Ron).Assembly);

    IDictionary<MemberInfo, string> membersToIDs = new MemberDictionary<string>();

    assembly.Accept(new ReverseIDVisitor(membersToIDs));

    assembly.Accept(new SignatureVisitor(assembly, membersToIDs));

    return assembly.Xml.ToString();
  }
}

internal class ReverseIDVisitor(IDictionary<MemberInfo, string> membersToIDs) : XmlVisitor
{
  public override void VisitMember(Member member)
  {
    membersToIDs.Add(member.Info.NotNull(nameof(member.Info)), member.Id);

    base.VisitMember(member);
  }
}

internal class SignatureVisitor(
  AssemblyMembers assemblyMembers,
  IDictionary<MemberInfo, string> membersToIDs
) : XmlVisitor
{
  public XDocument xml => assemblyMembers.Xml;

  public override void VisitClass(Class element)
  {
    VisitMember(element);
    var type = element.Info.AsNotNull<Type>();

    var xmlEl = XElement(element);

    if (type.BaseType is Type baseType && membersToIDs.TryGetValue(baseType, out var baseTypeId))
    {
      xmlEl.Add(new XAttribute("baseClass", baseTypeId));
    }
  }

  public override void VisitMethod(Method method)
  {
    VisitMember(method);

    var xmlEl = XElement(method);

    if (method.Info is not MethodInfo info)
    {
      return;
    }

    if (info.GetBaseOrInterfaceMethod() is MethodInfo baseMethod && baseMethod != info)
    {
      if (
        xmlEl.XPathSelectElement($"//member[@name='{method.Id}']//inheritdoc")
        is not XElement inheritdoc
      )
      {
        return;
      }

      AddBaseImpl(inheritdoc, baseMethod);
    }
  }

  public override void VisitProperty(Property property)
  {
    var info = property.Info.AsNotNull<PropertyInfo>();

    var xmlEl = XElement(property);

    if (info.GetBaseOrInterfaceProperty() is PropertyInfo baseProperty)
    {
      if (
        xmlEl.XPathSelectElement($"//member[@name='{property.Id}']//inheritdoc")
        is not XElement inheritdoc
      )
      {
        return;
      }

      AddBaseImpl(inheritdoc, baseProperty);
    }

    this.VisitMember(property);
  }

  public override void VisitMember(Member member)
  {
    AddSignature(member);
    AddDeclaringType(member);
    AddType(member);
    AddFriendlyName(member);
  }

  XElement XElement(Member member)
  {
    return xml.XPathSelectElement($"//member[@name='{member.Id}']").NotNull("xmlEl");
  }

  void AddDeclaringType(Member member)
  {
    var declaringType = member.Info?.DeclaringType ?? member.Info.AsNotNull<Type>();

    var xmlEl = XElement(member);
    if (membersToIDs.TryGetValue(declaringType, out var declaringTypeID))
    {
      xmlEl.Add(new XAttribute("declaringType", declaringTypeID));
    }
  }

  void AddSignature(Member member)
  {
    var xmlEl = XElement(member);

    if (
      MemberSignature.GetMemberSignature(member.Info.NotNull(nameof(member.Info)))
      is string signature
    )
    {
      xmlEl.Add(new XAttribute("signature", signature));
    }

    xmlEl.AddFirst(
      XMLSignature.GetMemberSignature(member.Info.NotNull(nameof(member.Info)), membersToIDs)
    );

    if (
      XMLSignature.GetSearchSignature(member.Info.NotNull(nameof(member.Info)), membersToIDs)
      is XNode searchSignatureNode
    )
    {
      xmlEl.Add(new XAttribute("searchSignature", searchSignatureNode.XMLValueText()));
      xmlEl.Add(new XAttribute("searchSignatureXml", searchSignatureNode.ToString() ?? ""));
    }
  }

  void AddType(Member member)
  {
    var info = member.Info;
    var xmlEl = XElement(member);
    var memberType = info switch
    {
      Type => "type",
      ConstructorInfo => "constructor",
      MethodInfo => "method",
      FieldInfo => "field",
      PropertyInfo => "property",
      _ => null,
    };
    if (memberType is string)
    {
      xmlEl.Add(new XAttribute("memberType", memberType));
    }
  }

  void AddFriendlyName(Member member)
  {
    var info = member.Info;
    var xmlEl = XElement(member);
    if (member.Info is ConstructorInfo constructor)
    {
      xmlEl.Add(
        new XAttribute(
          "friendlyName",
          Regex.Replace(info?.DeclaringType?.Name ?? "", "`.*", "") + ".ctor"
        )
      );
    }
    else
    {
      xmlEl.Add(new XAttribute("friendlyName", Regex.Replace(info?.Name ?? "", "`.*", "")));
    }
  }

  void AddBaseImpl(XElement node, MemberInfo baseMember)
  {
    if (membersToIDs.TryGetValue(baseMember, out var baseId))
    {
      node.Add(new XAttribute("baseImpl", baseId));
      if (ElementURL.GetElementUrl(baseMember, membersToIDs) is string elementUrl)
      {
        node.Add(new XAttribute("baseImplUrl", elementUrl));
      }
    }
  }
}
