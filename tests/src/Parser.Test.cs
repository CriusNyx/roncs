using DeepEqual.Syntax;
using RonCS;
using Superpower;
using static RonBuilder;

namespace RonTests;

public class ASTTests
{
  [Test]
  public void CanParse_None()
  {
    TestParser("None", None, RonParser.Option);
  }

  [Test]
  public void CanParse_Some()
  {
    TestParser("Some(None)", Some(None), RonParser.Option);
  }

  [Test]
  public void CanParse_UnitTuple()
  {
    TestParser("()", Tuple(), RonParser.Tuple);
  }

  [Test]
  public void CanParse_SingleTuple()
  {
    TestParser("(None)", Tuple(None), RonParser.Tuple);
  }

  [Test]
  public void CanParse_DoubleTuple()
  {
    TestParser("(None, None)", Tuple(None, None), RonParser.Tuple);
  }

  [Test]
  public void CanParse_TrailingComma_Tuple()
  {
    TestParser("(None,)", Tuple(None), RonParser.Tuple);
  }

  [Test]
  public void CanParse_TrailingComma_UnitTuple()
  {
    TestParser("(,)", Tuple(), RonParser.Tuple);
  }

  [Test]
  public void CarParse_Identifier()
  {
    TestParser("ident", Ident("ident"), RonParser.Identifier);
  }

  [Test]
  public void CanParse_RawIdentifier()
  {
    TestParser("r#raw+ident", RawIdent("r#raw+ident"), RonParser.Identifier);
  }

  [Test]
  public void CanParse_NamedValue()
  {
    TestParser("ident: None", NamedValue("ident", None), RonParser.NamedValue);
  }

  [Test]
  public void CanParse_NamedValueSet_Empty()
  {
    TestParser("", [], RonParser.NamedValueSet);
  }

  [Test]
  public void CanParse_NamedValueSet_Empty_TrailingComma()
  {
    TestParser(",", [], RonParser.NamedValueSet);
  }

  [Test]
  public void CanParse_NamedValueSet()
  {
    TestParser(
      "a: None, b: None",
      [NamedValue("a", None), NamedValue("b", None)],
      RonParser.NamedValueSet
    );
  }

  [Test]
  public void CanParse_NamedValueSet_TrailingComma()
  {
    TestParser(
      "a: None, b: None,",
      [NamedValue("a", None), NamedValue("b", None)],
      RonParser.NamedValueSet
    );
  }

  [Test]
  public void CanParse_UnitStruct()
  {
    TestParser("Struct", UnitStruct("Struct"), RonParser.Struct);
  }

  [Test]
  public void CanParse_True()
  {
    TestParser("true", Bool(true), RonParser.Boolean);
  }

  [Test]
  public void CanParse_False()
  {
    TestParser("false", Bool(false), RonParser.Boolean);
  }

  [Test]
  public void CanParse_MapItem()
  {
    TestParser("false: true", MapItem(Bool(false), Bool(true)), RonParser.MapItem);
  }

  [Test]
  public void CanParse_MapItemSet_Empty()
  {
    TestParser("", [], RonParser.MapItemSet);
  }

  [Test]
  public void CanParse_MapItemSet_Empty_TrailingComma()
  {
    TestParser(",", [], RonParser.MapItemSet);
  }

  [Test]
  public void CanParse_MapItemSet()
  {
    TestParser(
      "true: true, false: false",
      [MapItem(Bool(true), Bool(true)), MapItem(Bool(false), Bool(false))],
      RonParser.MapItemSet
    );
  }

  [Test]
  public void CanParse_Map_Empty()
  {
    TestParser("{ }", Map(), RonParser.Map);
  }

  [Test]
  public void CanParse_Map_Empty_TrailingComma()
  {
    TestParser("{,}", Map(), RonParser.Map);
  }

  [Test]
  public void CanParse_Map()
  {
    TestParser(
      "{ true: true, false: false }",
      Map(MapItem(Bool(true), Bool(true)), MapItem(Bool(false), Bool(false))),
      RonParser.Map
    );
  }

  [Test]
  public void CanParse_Map_TrailingComma()
  {
    TestParser(
      "{ true: true, false: false, }",
      Map(MapItem(Bool(true), Bool(true)), MapItem(Bool(false), Bool(false))),
      RonParser.Map
    );
  }

  [Test]
  public void CanParse_List_Empty()
  {
    TestParser("[]", List(), RonParser.List);
  }

  [Test]
  public void CanParse_List_Empty_TrailingComma()
  {
    TestParser("[,]", List(), RonParser.List);
  }

  [Test]
  public void CanParse_List()
  {
    TestParser("[true]", List(Bool(true)), RonParser.List);
  }

  [Test]
  public void CanParse_List_TrailingComma()
  {
    TestParser("[true,]", List(Bool(true)), RonParser.List);
  }

  [Test]
  public void CanParse_Range_Binary()
  {
    TestParser(
      "0..0",
      Range(
        new IntegerValue(null, new(null, "0")),
        RonRangeOperator.Exclusive,
        new IntegerValue(null, new(null, "0"))
      ),
      RonParser.Range
    );
  }

  [Test]
  public void CanParse_Range_Binary_Inclusive()
  {
    TestParser(
      "0..=0",
      Range(
        new IntegerValue(null, new(null, "0")),
        RonRangeOperator.Inclusive,
        new IntegerValue(null, new(null, "0"))
      ),
      RonParser.Range
    );
  }

  [Test]
  public void CanParse_Range_From()
  {
    TestParser(
      "0..",
      Range(new IntegerValue(null, new(null, "0")), RonRangeOperator.Exclusive, null),
      RonParser.Range
    );
  }

  [Test]
  public void CanParse_Range_To()
  {
    TestParser(
      "..0",
      Range(null, RonRangeOperator.Exclusive, new IntegerValue(null, new(null, "0"))),
      RonParser.Range
    );
  }

  [Test]
  public void CanParse_Range_To_Inclusive()
  {
    TestParser(
      "..=0",
      Range(null, RonRangeOperator.Inclusive, new IntegerValue(null, new(null, "0"))),
      RonParser.Range
    );
  }

  [Test]
  public void CanParse_Range_Full()
  {
    TestParser("..", Range(null, RonRangeOperator.Exclusive, null), RonParser.Range);
  }

  [Test]
  public void CanParse_Ron()
  {
    TestParser("Struct", Ron(UnitStruct("Struct")), RonParser.Ron);
  }

  [Test]
  public void CantParse_Tuple_DoubleTrailingComma()
  {
    Assert.Throws<ParseException>(() => RonParser.Tuple.Parse(RonLexer.Tokenize("(,,)")));
  }

  [Test]
  public void CantParse_NamedValueSet_DoubleTrailingComma()
  {
    Assert.Throws<ParseException>(() =>
      RonParser.NamedValueSet.AtEnd().Parse(RonLexer.Tokenize(",,"))
    );
  }

  [Test]
  public void CantParse_MapItemSet_DoubleTrailingComma()
  {
    Assert.Throws<ParseException>(() =>
      RonParser.MapItemSet.AtEnd().Parse(RonLexer.Tokenize(",,"))
    );
  }

  static void TestParser(
    string source,
    RonElement ast,
    TokenListParser<RonTokenKind, RonElement> parser
  )
  {
    parser = parser ?? RonParser.Ron;
    var tokens = RonLexer.Tokenize(source);
    var parsed = parser.Parse(tokens);

    parsed.ShouldDeepEqual(ast);
  }

  static void TestParser(
    string source,
    IEnumerable<RonElement> ast,
    TokenListParser<RonTokenKind, IEnumerable<RonElement>> parser
  )
  {
    var tokens = RonLexer.Tokenize(source);
    var parsed = parser.Parse(tokens);

    parsed.ShouldDeepEqual(ast);
  }
}
