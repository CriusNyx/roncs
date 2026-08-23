using System.Numerics;
using DeepEqual.Syntax;
using RonCS;

namespace RonTests;

public class ObjectDeserializerTests
{
  [OneTimeSetUp]
  public void Setup()
  {
    Ron.RegisterType(
      typeof(EmptyClass),
      typeof(SimpleClass),
      typeof(NestedClass),
      typeof(ParentClass),
      typeof(ChildClassA),
      typeof(ChildClassB),
      typeof(Vector3),
      typeof(VectorList),
      typeof(WithDict),
      typeof(CreateWithDict)
    );

    Ron.RegisterProxyType(typeof(DoesNotHaveProxyType), typeof(CustomProxyDoesNotHaveProxy));
  }

  [OneTimeTearDown]
  public void Teardown()
  {
    Ron.ResetGlobalContext();
  }

  public static IEnumerable<object[]> ObjectTestData
  {
    get
    {
      yield return ["object", typeof(object), new object()];
      yield return ["object()", typeof(object), new object()];
      yield return ["()", typeof(object), new object()];
      yield return ["EmptyClass", typeof(object), new EmptyClass()];
      yield return ["EmptyClass()", typeof(object), new EmptyClass()];
      yield return ["()", typeof(EmptyClass), new EmptyClass()];
      yield return
      [
        "SimpleClass( foo: \"string\", bar: -1 )",
        typeof(object),
        new SimpleClass { foo = "string", bar = -1 },
      ];
      yield return
      [
        "(foo: \"string\", bar: -1)",
        typeof(SimpleClass),
        new SimpleClass { foo = "string", bar = -1 },
      ];
      yield return
      [
        "NestedClass(simpleClass: SimpleClass( foo: \"string\", bar: -1 ))",
        typeof(object),
        new NestedClass
        {
          simpleClass = new SimpleClass { foo = "string", bar = -1 },
        },
      ];
      yield return
      [
        "NestedClass(simpleClass: ( foo: \"string\", bar: -1 ))",
        typeof(object),
        new NestedClass
        {
          simpleClass = new SimpleClass { foo = "string", bar = -1 },
        },
      ];
      yield return
      [
        "(simpleClass: ( foo: \"string\", bar: -1 ))",
        typeof(NestedClass),
        new NestedClass
        {
          simpleClass = new SimpleClass { foo = "string", bar = -1 },
        },
      ];
      yield return ["ChildClassA(foo: \"bar\")", typeof(object), new ChildClassA { foo = "bar" }];
      yield return ["ChildClassB(bar: \"baz\")", typeof(object), new ChildClassB { bar = "baz" }];
      yield return ["(foo: \"bar\")", typeof(ChildClassA), new ChildClassA { foo = "bar" }];
      yield return ["(bar: \"baz\")", typeof(ChildClassB), new ChildClassB { bar = "baz" }];
      yield return ["Vector3", typeof(object), new Vector3()];
      yield return ["Vector3()", typeof(object), new Vector3()];
      yield return ["()", typeof(Vector3), new Vector3()];
      yield return ["Vector3(1, 2, 3)", typeof(object), new Vector3(1, 2, 3)];
      yield return ["Vector3(0.1, 0.2, 0.3)", typeof(object), new Vector3(0.1f, 0.2f, 0.3f)];
      yield return ["(1, 2, 3)", typeof(Vector3), new Vector3(1, 2, 3)];
      yield return ["[]", typeof(object[]), new object[] { }];
      yield return ["[]", typeof(int[]), new object[] { }];
      yield return ["[]", typeof(IEnumerable<int>), new object[] { }];
      yield return ["[1, 2, 3]", typeof(int[]), new object[] { 1, 2, 3 }];
      yield return ["[1, 2, 3]", typeof(IEnumerable<int>), new object[] { 1, 2, 3 }];
      yield return
      [
        "[(), (1, 2, 3)]",
        typeof(Vector3[]),
        new object[] { new Vector3(), new Vector3(1, 2, 3) },
      ];
      yield return
      [
        "[(), (1, 2, 3)]",
        typeof(IEnumerable<Vector3>),
        new object[] { new Vector3(), new Vector3(1, 2, 3) },
      ];
      yield return
      [
        "(values: [(), (1, 2, 3)])",
        typeof(VectorList),
        new VectorList { values = [new Vector3(), new Vector3(1, 2, 3)] },
      ];
      yield return
      [
        "VectorList(values: [(), (1, 2, 3)])",
        typeof(object),
        new VectorList { values = [new Vector3(), new Vector3(1, 2, 3)] },
      ];
      yield return
      [
        "[(), (1, 2, 3)]",
        typeof(List<Vector3>),
        new Vector3[] { new Vector3(), new Vector3(1, 2, 3) },
      ];
      yield return
      [
        """{"Hello": "World"}""",
        typeof(Dictionary<string, string>),
        new Dictionary<string, string> { { "Hello", "World" } },
      ];
      yield return
      [
        """{"Hello": "World"}""",
        typeof(IDictionary<string, string>),
        new Dictionary<string, string> { { "Hello", "World" } },
      ];
      yield return
      [
        """(values: {"Hello": (1, 2, 3)})""",
        typeof(WithDict),
        new WithDict
        {
          values = new Dictionary<string, Vector3> { { "Hello", new Vector3(1, 2, 3) } },
        },
      ];
      yield return
      [
        """{"Hello": (1, 2, 3)}""",
        typeof(CreateWithDict),
        new CreateWithDict
        {
          values = new Dictionary<string, Vector3> { { "Hello", new Vector3(1, 2, 3) } },
        },
      ];
      yield return
      [
        """CreateWithDict{"Hello": (1, 2, 3)}""",
        typeof(object),
        new CreateWithDict
        {
          values = new Dictionary<string, Vector3> { { "Hello", new Vector3(1, 2, 3) } },
        },
      ];
      yield return ["""Some((1, 2, 3))""", typeof(Vector3), new Vector3(1, 2, 3)];
      yield return ["""None""", typeof(Vector3), null!];
      yield return ["""VectorTuple(1, 2, 3)""", typeof(VectorTuple), new VectorTuple(1, 2, 3)];
      yield return ["""(1, 2, 3)""", typeof(VectorTuple), new VectorTuple(1, 2, 3)];
      yield return
      [
        """DoesNotHaveProxyType(value:"(1, 2, 3)")""",
        typeof(DoesNotHaveProxyType),
        new DoesNotHaveProxyType { value = new Vector3(1, 2, 3) },
      ];
      yield return
      [
        "HasValue(value:Vector3(1,2,3))",
        typeof(HasValue),
        new HasValue { value = new Vector3(1, 2, 3) },
      ];
    }
  }

  [Theory]
  [TestCaseSource(nameof(ObjectTestData))]
  public void CanDeserializeObject(string ron, Type typeHint, object expected)
  {
    var actual = Ron.Deserialize(ron, typeHint);
    actual.ShouldDeepEqual(expected);
  }
}
