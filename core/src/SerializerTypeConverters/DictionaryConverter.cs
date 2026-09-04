using System.Collections;
using RonCS.AST;

namespace RonCS.Converters;

/// <summary>
/// Converter for a dictionary type.
/// </summary>
internal class DictionaryConverter : TypeSerializerConverter
{
  public RonElement ToAST(SerializationContext context, object source)
  {
    var elements = new List<RonElement>();
    var dict = source.RonConvert<IDictionary>();
    foreach (var key in dict.Keys)
    {
      var value = dict[key];
      elements.Add(
        new RonMapItem(
          new RonString(new RonStringLit(key.ToString().NotNull())),
          context.ToAST(value)
        )
      );
    }
    return new RonMap(elements.ToArray());
  }
}
