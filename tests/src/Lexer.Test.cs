using RonCS;
using RonCS.AST;

namespace RonTests;

public class LexerTests
{
  // Whitespace
  [TestCase("", new RonTokenKind[] { })]
  [TestCase(" ", new RonTokenKind[] { })]
  [TestCase("\n", new RonTokenKind[] { })]
  [TestCase("\r", new RonTokenKind[] { })]
  // Comments
  [TestCase("//", new RonTokenKind[] { })]
  [TestCase("//\nabc", new RonTokenKind[] { RonTokenKind.Identifier })]
  // Block Comments
  [TestCase("/**/", new RonTokenKind[] { })]
  [TestCase("/* */", new RonTokenKind[] { })]
  [TestCase("/*/**/*/", new RonTokenKind[] { })]
  [TestCase("/* /* */ */", new RonTokenKind[] { })]
  [TestCase("/* /* \n */ */", new RonTokenKind[] { })]
  [TestCase("/**/abc123", new RonTokenKind[] { RonTokenKind.Identifier })]
  [TestCase("/*\n*/abc123", new RonTokenKind[] { RonTokenKind.Identifier })]
  [TestCase("/*abc*/", new RonTokenKind[] { })]
  [TestCase("/*abc\n*/", new RonTokenKind[] { })]
  [TestCase("/*abc\n123*/", new RonTokenKind[] { })]
  // Boolean
  [TestCase("true", new RonTokenKind[] { RonTokenKind.True })]
  [TestCase("false", new RonTokenKind[] { RonTokenKind.False })]
  // Options
  [TestCase("Some", new RonTokenKind[] { RonTokenKind.Some })]
  [TestCase("None", new RonTokenKind[] { RonTokenKind.None })]
  // Identifier
  [TestCase("a", new RonTokenKind[] { RonTokenKind.Identifier })]
  [TestCase("abc123", new RonTokenKind[] { RonTokenKind.Identifier })]
  [TestCase("abc_123", new RonTokenKind[] { RonTokenKind.Identifier })]
  [TestCase("_abc", new RonTokenKind[] { RonTokenKind.Identifier })]
  [TestCase("a b", new RonTokenKind[] { RonTokenKind.Identifier, RonTokenKind.Identifier })]
  // Raw Identifier
  [TestCase("r#abc", new RonTokenKind[] { RonTokenKind.RawIdentifier })]
  [TestCase("r#abc-123", new RonTokenKind[] { RonTokenKind.RawIdentifier })]
  [TestCase("r#abc.123", new RonTokenKind[] { RonTokenKind.RawIdentifier })]
  [TestCase("r#abc+123", new RonTokenKind[] { RonTokenKind.RawIdentifier })]
  // Characters
  [TestCase("'a'", new RonTokenKind[] { RonTokenKind.Char })]
  [TestCase("'\\''", new RonTokenKind[] { RonTokenKind.Char })]
  [TestCase("'\\\\'", new RonTokenKind[] { RonTokenKind.Char })]
  public void TokenizerMatch(string source, RonTokenKind[] kinds)
  {
    var lexed = RonLexer.Tokenizer.Tokenize(source);
    Assert.That(lexed.Select(x => x.Kind), Is.EquivalentTo(kinds));
  }

  [TestCase("abc123!")]
  [TestCase("/*")]
  public void ShouldNotTokenize(string source)
  {
    Assert.Throws<Superpower.ParseException>(() => RonLexer.Tokenizer.Tokenize(source));
  }
}
