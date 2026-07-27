using CriusNyx.Util;
using Superpower;
using Superpower.Parsers;

namespace Ron;

public static class TriviaParser
{
  public static TextParser<TriviaElement> whitespace_Parser =
    from start in MoreParsers.Position
    from whitespace in Character.WhiteSpace.AsString()
    from end in MoreParsers.Position
    select new Whitespace { span = RonSpan.From(start, end), text = whitespace } as TriviaElement;

  public static TextParser<TriviaElement> LineComment_Parser =
    from start in MoreParsers.Position
    from leading in Span.EqualTo("//")
    from rest in Character.Except('\n').AsString()
    from end in MoreParsers.Position
    select new Comment
    {
      span = RonSpan.From(start, end),
      text = leading + rest,
      kind = CommentKind.Line,
    } as TriviaElement;

  public static TextParser<string> BlockComment_Content = Parse.Ref(() =>
    Parse.OneOf(
      BlockCommentLit_Parser.NotNull("BlockCommentLit").Try(),
      BlockCommentText_Parser.NotNull("BlockCommentText")
    )
  );

  public static TextParser<string> BlockCommentLit_Parser = MoreParsers.Until(
    Span.EqualTo("/*"),
    Span.EqualTo("*/")
  );

  public static TextParser<string> BlockCommentText_Parser =
    from opening in Span.EqualTo("/*")
    from inner in BlockComment_Content.Many()
    from closing in Span.EqualTo("*/")
    select opening + inner.StringJoin() + closing;

  public static TextParser<TriviaElement> BLockComment_Parser =
    from start in MoreParsers.Position
    from text in BlockCommentText_Parser
    from end in MoreParsers.Position
    select new Comment
    {
      span = RonSpan.From(start, end),
      text = text,
      kind = CommentKind.Block,
    } as TriviaElement;

  public static TextParser<Trivia> Trivia_Parser =
    from start in MoreParsers.Position
    from elements in Parse
      .OneOf(whitespace_Parser, LineComment_Parser.Try(), BLockComment_Parser)
      .Many()
    from end in MoreParsers.Position
    select new Trivia { span = RonSpan.From(start, end), elements = elements };
}
