using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using CriusNyx.Util;

public static class RouteCacheGenerator
{
  public static void GenerateRouteCacheFile(string file)
  {
    File.WriteAllText(file, GenerateRouteCache());
  }

  public static string GenerateRouteCache()
  {
    var xmlCache = XMLDocGenerator.GenerateXMLDocs();
    var xDoc = XDocument.Parse(xmlCache);

    var members = xDoc.XPathSelectElements("//member")
      .WhereAs<XElement>()
      .Select(RouteFromXElement)
      .WhereAs<Route>()
      .ToDictionary(x => x.id);

    foreach (var member in members.Values)
    {
      if (member.declaringType != null && member.declaringType != member.id)
      {
        var parent = members[member.declaringType];
        parent.children.Add(member);
      }
    }

    var rootMembers = members
      .Values.Where(x => x.declaringType == null || x.declaringType == x.id)
      .OrderBy(x => x.name);

    var json = JsonSerializer.Serialize(
      rootMembers,
      new JsonSerializerOptions { WriteIndented = true }
    );

    return json;
  }

  private static Route? RouteFromXElement(XElement element)
  {
    var memberType = element.GetAttribute("memberType").NotNull("memberType");

    if (memberType != "type")
    {
      return null;
    }

    var friendlyName = element.GetAttribute("friendlyName").NotNull("friendlyName");
    var declaringType = element.GetAttribute("declaringType");
    var name = element.GetAttribute("name").NotNull();

    string UrlEncodeUpperCase(string stringToEncode)
    {
      var reg = new Regex(@"%[a-f0-9]{2}");
      stringToEncode = HttpUtility.UrlEncode(stringToEncode);
      return reg.Replace(stringToEncode, m => m.Value.ToUpperInvariant());
    }

    string GenerateHRef()
    {
      if (declaringType != null && declaringType != name)
      {
        return $"/types/{UrlEncodeUpperCase(declaringType)}#{UrlEncodeUpperCase(name)}";
      }
      else
      {
        return $"/types/{UrlEncodeUpperCase(name)}";
      }
    }

    return new Route
    {
      id = name,
      declaringType = declaringType,
      name = friendlyName,
      href = GenerateHRef(),
    };
  }
}

[DebugPrint(auto: true)]
internal class Route
{
  public string id = null!;
  public string? declaringType;

  [JsonInclude]
  public string name = null!;

  [JsonInclude]
  public string href = null!;

  [JsonInclude]
  public List<Route> children = new List<Route>();
}
