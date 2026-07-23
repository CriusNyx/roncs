using CriusNyx.Util;
using DeepEqual.Syntax;
using Superpower;

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
    var expected = new RonDocument(new StringValue([new AsciiEscape(escaped)]));
    var parsed = RonParser.Ron.Parse(RonLexer.Tokenize(source));
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
    var expected = new RonDocument(new StringValue([new ByteEscape(hex[0], hex[1])]));
    var parsed = RonParser.Ron.Parse(RonLexer.Tokenize(source));
    expected.ShouldDeepEqual(parsed);
  }

  [Test]
  public void CanParseEmptyString()
  {
    string source = $"\"\"";
    var expected = new RonDocument(new StringValue([]));
    var parsed = RonParser.Ron.Parse(RonLexer.Tokenize(source));
    expected.ShouldDeepEqual(parsed);
  }

  [Theory]
  [TestCase("Hello")]
  [TestCase("world")]
  public void CanParseStandardString(string content)
  {
    string source = $"\"{content}\"";
    var expected = new RonDocument(new StringValue([new StringLit(content)]));
    var parsed = RonParser.Ron.Parse(RonLexer.Tokenize(source));
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
    var expected = new RonDocument(new StringValue([new StringRawLit(content)]));
    var parsed = RonParser.Ron.Parse(RonLexer.Tokenize(source));
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
      new StringValue([new StringRawContent(new StringRawLit(content))])
    );
    var parsed = RonParser.Ron.Parse(RonLexer.Tokenize(source));
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
      new StringValue([new StringRawContent(new StringRawContent(new StringRawLit(content)))])
    );
    var parsed = RonParser.Ron.Parse(RonLexer.Tokenize(source));
    expected.ShouldDeepEqual(parsed);
  }

  [Theory]
  public void CanParseUnicodeString(string source) { }

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
    var parsed = RonParser.Ron.Parse(RonLexer.Tokenize(source));
    var stringVal = parsed.As<RonDocument>()?.Value?.As<StringValue>()?.Evaluate();
    Assert.That(stringVal, Is.EqualTo(expected));
  }
}
