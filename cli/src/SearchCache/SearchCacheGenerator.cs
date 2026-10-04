using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using CriusNyx.Util;

public static class SearchCacheGenerator
{
  public static void GenerateSearchCacheFile(string filePath)
  {
    File.WriteAllText(filePath, GenerateSearchCache());
  }

  public static string GenerateSearchCache()
  {
    var xml = XMLDocGenerator.GenerateXMLDocs();
    var xDocument = XDocument.Parse(xml);

    var members = xDocument.XPathSelectElements("//member");
    var searchCache = members.Select(SearchCacheElementFromXElement);

    var json = JsonSerializer.Serialize(
      searchCache,
      new JsonSerializerOptions { WriteIndented = true }
    );
    return json;
  }

  static SearchCacheElement SearchCacheElementFromXElement(XElement member)
  {
    return new SearchCacheElement
    {
      name = member.GetAttribute("name").NotNull(),
      declaringType = member.GetAttribute("declaringType"),
      shortName = member.GetAttribute("friendlyName").NotNull(),
      signature = member.GetAttribute("searchSignature").NotNull(),
      signatureXml = member.GetAttribute("searchSignatureXml").NotNull(),
      summary = member.XPathLocal("//summary").FirstOrDefault()?.As<XElement>()?.Value.Trim(),
    };
  }
}

[DebugPrint(auto: true)]
public class SearchCacheElement
{
  [JsonInclude]
  public string name = null!;

  [JsonInclude]
  public string? declaringType;

  [JsonInclude]
  public string shortName = null!;

  [JsonInclude]
  public string signature = null!;

  [JsonInclude]
  public string? signatureXml;

  [JsonInclude]
  public string? summary;
}
