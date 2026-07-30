using CriusNyx.Util;
using Superpower;
using Superpower.Model;

namespace RonCS;

public static partial class Ron
{
  public static TokenList<RonTokenKind> Tokenize(string source)
  {
    return RonLexer.Tokenize(source);
  }

  public static RonDocument Parse(TokenList<RonTokenKind> tokenList)
  {
    return RonParser.Ron.Parse(tokenList).AsNotNull<RonDocument>("Parsed");
  }

  public static RonDocument Parse(string source)
  {
    return Parse(Tokenize(source));
  }

  public static object Deserialize(string source, Type? hint)
  {
    var document = Parse(source);
    var deserialized = RonSerializer.Deserialize(document, hint);
    return deserialized;
  }
}
