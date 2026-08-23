using System.Collections;
using RonCS.AST;

namespace RonCS;

internal class ListConverter : TypeSerializerConverter
{
  public RonElement ToAST(SerializationContext context, object source)
  {
    var list = source.RonConvert<IEnumerable>().Cast<object>();
    var elements = list.Select(element => context.ToAST(element));
    return new RonList(elements.ToArray());
  }
}
