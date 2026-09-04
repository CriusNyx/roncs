using CriusNyx.Util;
using DeepEqual.Syntax;
using RonCS;
using RonCS.AST;
using Superpower;

namespace RonTests;

public class StringParserTests
{
  [Theory]
  [TestCase('\'')]
  [TestCase('"')]
  [TestCase('\\')]
  [TestCase('n')]
  [TestCase('r')]
  [TestCase('t')]
  [TestCase('0')]
  public void CanParseAsciiEscape(char escaped)
  {
    string source = $"\"\\{escaped}\"";
    var expected = new RonDocument(new RonString([new RonAsciiEscape(escaped)]));
    var parsed = Ron.Parse(source);
    expected.ShouldDeepEqual(parsed);
  }

  [Theory]
  [TestCase("00")]
  [TestCase("09")]
  [TestCase("A0")]
  [TestCase("a0")]
  [TestCase("FF")]
  [TestCase("ff")]
  [TestCase("Ff")]
  [TestCase("fF")]
  public void CanParseHexEscape(string hex)
  {
    string source = $"\"\\x{hex}\"";
    var expected = new RonDocument(new RonString([new RonByteEscape(hex[0], hex[1])]));
    var parsed = Ron.Parse(source);
    expected.ShouldDeepEqual(parsed);
  }

  [Test]
  public void CanParseEmptyString()
  {
    string source = $"\"\"";
    var expected = new RonDocument(new RonString([]));
    var parsed = Ron.Parse(source);
    expected.ShouldDeepEqual(parsed);
  }

  [Theory]
  [TestCase("Hello")]
  [TestCase("world")]
  public void CanParseStandardString(string content)
  {
    string source = $"\"{content}\"";
    var expected = new RonDocument(new RonString([new RonStringLit(content)]));
    var parsed = Ron.Parse(source);
    expected.ShouldDeepEqual(parsed);
  }

  [Theory]
  [TestCase("")]
  [TestCase("Hello")]
  [TestCase("\\")]
  [TestCase("\\r\\n")]
  [TestCase("\\xFF")]
  public void CanParseRawStringLit(string content)
  {
    string source = $"r\"{content}\"";
    var expected = new RonDocument(new RonString([new RonStringRawLit(content)]));
    var parsed = Ron.Parse(source);
    expected.ShouldDeepEqual(parsed);
  }

  [Theory]
  [TestCase("")]
  [TestCase("Hello")]
  [TestCase("\\")]
  [TestCase("\\r\\n")]
  [TestCase("\\xFF")]
  public void CanParseEmbeddedRawStringLit(string content)
  {
    string source = $"r#\"{content}\"#";
    var expected = new RonDocument(
      new RonString([new RonStringRawContent(new RonStringRawLit(content))])
    );
    var parsed = Ron.Parse(source);
    expected.ShouldDeepEqual(parsed);
  }

  [Theory]
  [TestCase("")]
  [TestCase("Hello")]
  [TestCase("\\")]
  [TestCase("\\r\\n")]
  [TestCase("\\xFF")]
  public void CanParseDoubleEmbeddedRawStringLit(string content)
  {
    string source = $"r##\"{content}\"##";
    var expected = new RonDocument(
      new RonString([
        new RonStringRawContent(new RonStringRawContent(new RonStringRawLit(content))),
      ])
    );
    var parsed = Ron.Parse(source);
    expected.ShouldDeepEqual(parsed);
  }

  [Theory]
  [TestCase("\"\"", "")]
  [TestCase("\"Hello\"", "Hello")]
  [TestCase("\"\\u{41}\"", "A")]
  [TestCase("\"\\u{1F339}\"", "🌹")]
  [TestCase("\"\\u{4F55}\"", "何")]
  [TestCase("\"\\'\"", "'")]
  [TestCase("\"\\\"\"", "\"")]
  [TestCase("\"\\\\\"", "\\")]
  [TestCase("\"\\n\"", "\n")]
  [TestCase("\"\\r\"", "\r")]
  [TestCase("\"\\t\"", "\t")]
  [TestCase("\"\\0\"", "\0")]
  [TestCase("r\"\\0\"", "\\0")]
  [TestCase("r#\"\\0\"#", "\\0")]
  public void CanDecodeString(string source, string expected)
  {
    var parsed = Ron.Parse(source);
    var stringVal = parsed.As<RonDocument>()?.Value?.As<RonString>()?.Evaluate();
    Assert.That(stringVal, Is.EqualTo(expected));
  }

  [Theory]
  [TestCase("\'a\'", 'a')]
  [TestCase("\'\\\'\'", '\'')]
  [TestCase("\'\\\\\'", '\\')]
  public void CanParseChar(string source, char expected)
  {
    var parsed = Ron.Parse(source);
    var c = parsed.Value.AsNotNull<RonChar>().Value;
    Assert.That(c, Is.EqualTo(expected));
  }
}
