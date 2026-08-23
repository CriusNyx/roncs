using DeepEqual.Syntax;
using RonCS;
using RonCS.AST;

namespace RonTests;

public class RonPrintTests
{
  public static IEnumerable<object[]> CanRonPrintASTData
  {
    get
    {
      yield return [new RonBool(false), RonPrintOptions.Compact(), "false"];
      yield return [new RonBool(true), RonPrintOptions.Compact(), "true"];
      yield return
      [
        new StringValue(new StringLit("Hello")),
        RonPrintOptions.Compact(),
        "\"Hello\"",
      ];
      yield return [new StringValue(new AsciiEscape('\\')), RonPrintOptions.Compact(), "\"\\\\\""];
      yield return
      [
        new StringValue(new UnicodeEscape("1F339")),
        RonPrintOptions.Compact(),
        "\"\\u{1F339}\"",
      ];
      yield return [new IntegerValue(null, new(null, "1"), null), RonPrintOptions.Compact(), "1"];
      yield return [new IntegerValue('+', new(null, "1"), null), RonPrintOptions.Compact(), "+1"];
      yield return [new IntegerValue('-', new(null, "1"), null), RonPrintOptions.Compact(), "-1"];
      yield return
      [
        new IntegerValue(null, new(null, "1"), IntegerSuffix.u32),
        RonPrintOptions.Compact(),
        "1u32",
      ];
      yield return
      [
        new IntegerValue('-', new(null, "1"), IntegerSuffix.i32),
        RonPrintOptions.Compact(),
        "-1i32",
      ];
      yield return
      [
        new FloatValue(null, new StandardFloatNum("1.", null), null),
        RonPrintOptions.Compact(),
        "1.",
      ];
      yield return
      [
        new FloatValue(null, new StandardFloatNum(".1", null), null),
        RonPrintOptions.Compact(),
        ".1",
      ];
      yield return
      [
        new FloatValue(null, new StandardFloatNum("0.1", null), null),
        RonPrintOptions.Compact(),
        "0.1",
      ];
      yield return
      [
        new FloatValue(null, new StandardFloatNum("0.0", null), null),
        RonPrintOptions.Compact(),
        "0.0",
      ];
      yield return
      [
        new FloatValue(null, new StandardFloatNum("1.0", null), null),
        RonPrintOptions.Compact(),
        "1.0",
      ];
      yield return
      [
        new FloatValue('+', new StandardFloatNum("1.0", null), null),
        RonPrintOptions.Compact(),
        "+1.0",
      ];
      yield return
      [
        new FloatValue('-', new StandardFloatNum("1.0", null), null),
        RonPrintOptions.Compact(),
        "-1.0",
      ];
      yield return
      [
        new FloatValue(null, new StandardFloatNum("1", new('e', null, "12")), null),
        RonPrintOptions.Compact(),
        "1e12",
      ];
      yield return
      [
        new FloatValue(null, new StandardFloatNum("1", new('e', '-', "12")), null),
        RonPrintOptions.Compact(),
        "1e-12",
      ];
      yield return
      [
        new FloatValue(null, new StandardFloatNum("1", null), FloatSuffix.f32),
        RonPrintOptions.Compact(),
        "1f32",
      ];
      yield return
      [
        new FloatValue('-', new StandardFloatNum("1", new('e', '-', "12")), FloatSuffix.f32),
        RonPrintOptions.Compact(),
        "-1e-12f32",
      ];
      // lists
      yield return [new RonList(), RonPrintOptions.Compact(), "[]"];
      yield return
      [
        new RonList(new StringValue(new StringLit("Hello World"))),
        RonPrintOptions.Compact(),
        "[\"Hello World\"]",
      ];
      yield return
      [
        new RonList(new StringValue(new StringLit("Hello World"))),
        RonPrintOptions.Pretty(),
        "[\n  \"Hello World\"\n]",
      ];
      yield return
      [
        new RonList(
          new StringValue(new StringLit("Hello")),
          new StringValue(new StringLit("World"))
        ),
        RonPrintOptions.Compact(),
        "[\"Hello\",\"World\"]",
      ];
      yield return
      [
        new RonList(
          new StringValue(new StringLit("Hello")),
          new StringValue(new StringLit("World"))
        ),
        RonPrintOptions.Pretty(),
        "[\n  \"Hello\",\n  \"World\"\n]",
      ];
      // Tuples
      yield return [new RonTuple(), RonPrintOptions.Compact(), "()"];
      yield return
      [
        new RonTuple(new StringValue(new StringLit("Hello"))),
        RonPrintOptions.Compact(),
        "(\"Hello\")",
      ];
      yield return
      [
        new RonTuple(new StringValue(new StringLit("Hello"))),
        RonPrintOptions.Pretty(),
        "(\n  \"Hello\"\n)",
      ];
      yield return
      [
        new RonTuple(
          new StringValue(new StringLit("Hello")),
          new StringValue(new StringLit("World"))
        ),
        RonPrintOptions.Compact(),
        "(\"Hello\",\"World\")",
      ];
      yield return
      [
        new RonTuple(
          new StringValue(new StringLit("Hello")),
          new StringValue(new StringLit("World"))
        ),
        RonPrintOptions.Pretty(),
        "(\n  \"Hello\",\n  \"World\"\n)",
      ];
      // Tuple Struct
      yield return
      [
        new RonTupleStruct(new RonIdentifier("MyCoolClass"), new RonTuple()),
        RonPrintOptions.Compact(),
        "MyCoolClass()",
      ];
      yield return
      [
        new RonUnitStruct(new RonIdentifier("MyCoolClass")),
        RonPrintOptions.Compact(),
        "MyCoolClass",
      ];
      yield return
      [
        new RonTupleStruct(
          new RonIdentifier("MyCoolClass"),
          new RonTuple(new StringValue(new StringLit("Hello World")))
        ),
        RonPrintOptions.Compact(),
        "MyCoolClass(\"Hello World\")",
      ];
      // Named Value Struct
      yield return
      [
        new RonNamedValueStruct(
          null,
          new RonNamedValue(new RonIdentifier("foo"), new StringValue(new StringLit("bar")))
        ),
        RonPrintOptions.Compact(),
        "(foo:\"bar\")",
      ];
      yield return
      [
        new RonNamedValueStruct(
          null,
          new RonNamedValue(new RonIdentifier("foo"), new StringValue(new StringLit("bar")))
        ),
        RonPrintOptions.Pretty(),
        "(\n  foo: \"bar\"\n)",
      ];
      yield return
      [
        new RonNamedValueStruct(
          new RonIdentifier("MyCoolClass"),
          new RonNamedValue(new RonIdentifier("foo"), new StringValue(new StringLit("bar")))
        ),
        RonPrintOptions.Compact(),
        "MyCoolClass(foo:\"bar\")",
      ];
      yield return
      [
        new RonNamedValueStruct(
          new RonIdentifier("MyCoolClass"),
          new RonNamedValue(new RonIdentifier("foo"), new StringValue(new StringLit("bar")))
        ),
        RonPrintOptions.Pretty(),
        "MyCoolClass(\n  foo: \"bar\"\n)",
      ];
      // Map
      yield return [new RonMapStruct(null, new RonMap()), RonPrintOptions.Compact(), "{}"];
      yield return
      [
        new RonMapStruct(new RonIdentifier("MyCoolMap"), new RonMap()),
        RonPrintOptions.Compact(),
        "MyCoolMap{}",
      ];
      yield return
      [
        new RonMapStruct(
          null,
          new RonMap(
            new RonMapItem(
              new StringValue(new StringLit("hello")),
              new StringValue(new StringLit("world"))
            )
          )
        ),
        RonPrintOptions.Compact(),
        "{\"hello\":\"world\"}",
      ];
      yield return
      [
        new RonMapStruct(
          null,
          new RonMap(
            new RonMapItem(
              new StringValue(new StringLit("hello")),
              new StringValue(new StringLit("world"))
            )
          )
        ),
        RonPrintOptions.Pretty(),
        "{\n  \"hello\": \"world\"\n}",
      ];
      yield return
      [
        new RonMapStruct(
          new RonIdentifier("MyCoolMap"),
          new RonMap(
            new RonMapItem(
              new StringValue(new StringLit("hello")),
              new StringValue(new StringLit("world"))
            )
          )
        ),
        RonPrintOptions.Compact(),
        "MyCoolMap{\"hello\":\"world\"}",
      ];
      yield return
      [
        new RonMapStruct(
          null,
          new RonMap(
            new RonMapItem(
              new StringValue(new StringLit("hello")),
              new StringValue(new StringLit("world"))
            ),
            new RonMapItem(
              new StringValue(new StringLit("foo")),
              new StringValue(new StringLit("bar"))
            )
          )
        ),
        RonPrintOptions.Compact(),
        "{\"hello\":\"world\",\"foo\":\"bar\"}",
      ];
      yield return
      [
        new RonMapStruct(
          null,
          new RonMap(
            new RonMapItem(
              new StringValue(new StringLit("hello")),
              new StringValue(new StringLit("world"))
            ),
            new RonMapItem(
              new StringValue(new StringLit("foo")),
              new StringValue(new StringLit("bar"))
            )
          )
        ),
        RonPrintOptions.Pretty(),
        "{\n  \"hello\": \"world\",\n  \"foo\": \"bar\"\n}",
      ];
    }
  }

  [Theory]
  [TestCaseSource(nameof(CanRonPrintASTData))]
  public void CanRonPrintAST(RonElement ast, RonPrintOptions options, string expected)
  {
    var actual = ast.RonPrint(options);
    Assert.That(actual, Is.EqualTo(expected));
    var doc = new RonDocument(ast);
    var parsed = Ron.Parse(actual);
    doc.ShouldDeepEqual(parsed);
    var actual2 = parsed.RonPrint(options);
    Assert.That(actual2, Is.EqualTo(expected));
  }
}
