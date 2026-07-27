using Ron;
using Superpower;
using Superpower.Parsers;

public static class RonParser
{
  public static TextParser<Expr> Expression_Parser = Parse.Return(null as Expr)!;
  public static TextParser<TupleElement> TupleElement_Parser = from start in MoreParsers.Position from leading in TriviaParser.Trivia_Parser from expr in 

  public static TextParser<UnitExpr> Unit_Expr =
    from start in MoreParsers.Position
    from leftParen in Character.EqualTo('(')
    from trivia in TriviaParser.Trivia_Parser
    from rightParent in Character.EqualTo(')')
    from end in MoreParsers.Position
    select new UnitExpr { span = RonSpan.From(start, end), trivia = trivia };

  public static TextParser<Ident> Ident_Parser =
    from start in MoreParsers.Position
    from name in Character
      .Matching(XID.IsXidStart, "XidStart")
      .Then(s =>
        Character.Matching(XID.IsXidContinue, "XidContinue").AsString().Select(rest => s + rest)
      )
    from end in MoreParsers.Position
    select new Ident { span = RonSpan.From(start, end), name = name };
}
