using CriusNyx.Util;
using Superpower;

namespace Ron.Tests;

public class TriviaTests
{
  public static IEnumerable<object[]> TriviaTestCases
  {
    get
    {
      yield return ["", new Trivia { elements = [] }];
      yield return [" ", new Trivia { elements = [new Whitespace { text = " " }] }];
      yield return ["\n", new Trivia { elements = [new Whitespace { text = "\n" }] }];
      yield return ["\t", new Trivia { elements = [new Whitespace { text = "\t" }] }];
      yield return
      [
        "// Hello World",
        new Trivia
        {
          elements = [new Comment { text = "// Hello World", kind = CommentKind.Line }],
        },
      ];

      yield return
      [
        "// Hello World\n  ",
        new Trivia
        {
          elements =
          [
            new Comment { text = "// Hello World", kind = CommentKind.Line },
            new Whitespace { text = "\n  " },
          ],
        },
      ];
      yield return
      [
        "//Hello\n//World\n",
        new Trivia
        {
          elements =
          [
            new Comment { text = "//Hello", kind = CommentKind.Line },
            new Whitespace { text = "\n" },
            new Comment { text = "//World", kind = CommentKind.Line },
            new Whitespace { text = "\n" },
          ],
        },
      ];
      yield return
      [
        "//Hello\n\n//World",
        new Trivia
        {
          elements =
          [
            new Comment { text = "//Hello", kind = CommentKind.Line },
            new Whitespace { text = "\n\n" },
            new Comment { text = "//World", kind = CommentKind.Line },
          ],
        },
      ];
      yield return
      [
        "/**/",
        new Trivia { elements = [new Comment { text = "/**/", kind = CommentKind.Block }] },
      ];
      yield return
      [
        "/*Hello World*/",
        new Trivia
        {
          elements = [new Comment { text = "/*Hello World*/", kind = CommentKind.Block }],
        },
      ];
      yield return
      [
        "/*Hello\nWorld*/",
        new Trivia
        {
          elements = [new Comment { text = "/*Hello\nWorld*/", kind = CommentKind.Block }],
        },
      ];
      yield return
      [
        "/*Hello*//*World*/",
        new Trivia
        {
          elements =
          [
            new Comment { text = "/*Hello*/", kind = CommentKind.Block },
            new Comment { text = "/*World*/", kind = CommentKind.Block },
          ],
        },
      ];
      yield return
      [
        "/*Hello*/\n/*World*/",
        new Trivia
        {
          elements =
          [
            new Comment { text = "/*Hello*/", kind = CommentKind.Block },
            new Whitespace { text = "\n" },
            new Comment { text = "/*World*/", kind = CommentKind.Block },
          ],
        },
      ];
      yield return
      [
        "/*Hello*///Bar\n/*World*/",
        new Trivia
        {
          elements =
          [
            new Comment { text = "/*Hello*/", kind = CommentKind.Block },
            new Comment { text = "//Bar", kind = CommentKind.Line },
            new Whitespace { text = "\n" },
            new Comment { text = "/*World*/", kind = CommentKind.Block },
          ],
        },
      ];
      yield return
      [
        "/*/*Hello*/*/",
        new Trivia
        {
          elements = [new Comment { text = "/*/*Hello*/*/", kind = CommentKind.Block }],
        },
      ];
      yield return
      [
        "//Hello /* World",
        new Trivia
        {
          elements = [new Comment { text = "//Hello /* World", kind = CommentKind.Line }],
        },
      ];
      yield return
      [
        "//Hello /* World */ Bar",
        new Trivia
        {
          elements = [new Comment { text = "//Hello /* World */ Bar", kind = CommentKind.Line }],
        },
      ];
    }
  }

  [Theory]
  [TestCaseSource(nameof(TriviaTestCases))]
  public void CanParseTrivia(string source, Trivia expected)
  {
    var actual = TriviaParser.Trivia_Parser.AtEnd().Parse(source);
    Assert.That(actual.Debug(), Is.EqualTo(expected.Debug()));
  }
}
