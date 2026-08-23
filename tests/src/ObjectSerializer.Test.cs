using DeepEqual.Syntax;
using RonCS;

namespace RonTests;

public class ObjectSerializerTests
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

  public static IEnumerable<object[]> ObjectSerializerTestData
  {
    get
    {
      yield return
      [
        new SimpleClass { foo = "Hello", bar = -1 },
        "SimpleClass(foo:\"Hello\",bar:-1)",
      ];
      yield return [new EmptyClass { }, "EmptyClass"];
      yield return [new NestedClass { simpleClass = null! }, "NestedClass(simpleClass:None)"];
      yield return
      [
        new NestedClass
        {
          simpleClass = new SimpleClass { foo = "Hello", bar = -1 },
        },
        "NestedClass(simpleClass:SimpleClass(foo:\"Hello\",bar:-1))",
      ];
      yield return [new ChildClassA { foo = "Hello" }, "ChildClassA(foo:\"Hello\")"];
      yield return [new ChildClassB { bar = "World" }, "ChildClassB(bar:\"World\")"];
      yield return [new Vector3(1, 2, 3), "Vector3(x:1,y:2,z:3)"];
      yield return
      [
        new VectorList { values = [new Vector3(1, 2, 3)] },
        "VectorList(values:[Vector3(x:1,y:2,z:3)])",
      ];
      yield return [new Vector3[] { new Vector3(1, 2, 3) }, "[Vector3(x:1,y:2,z:3)]"];
      yield return
      [
        new Dictionary<string, Vector3> { { "key", new Vector3(1, 2, 3) } },
        "{\"key\":Vector3(x:1,y:2,z:3)}",
      ];
      yield return
      [
        new TypeWithProxy { vector = new Vector3(1, 2, 3) },
        "TypeWithProxy(vectorValue:\"(1, 2, 3)\")",
      ];
      yield return [new StringList("Hello", new("World")), "[\"Hello\",\"World\"]"];
      yield return [new PropertyClass { Value = "Foo" }, "PropertyClass(Value:\"Foo\")"];
      yield return
      [
        new VectorPropertyClass { Value = new Vector3(1, 2, 3) },
        "VectorPropertyClass(Value:Vector3(x:1,y:2,z:3))",
      ];
      yield return
      [
        new StringBackedVector { Value = new Vector3(1, 2, 3) },
        "StringBackedVector(value:\"(1, 2, 3)\")",
      ];
      yield return
      [
        NotRonList.From(["Hello", "World"])!,
        "NotRonList(value:\"Hello\",next:NotRonList(value:\"World\",next:None))",
      ];
      yield return
      [
        new HasEnumerable { values = NotRonList.From(["Hello", "World"])! },
        "HasEnumerable(values:[\"Hello\",\"World\"])",
      ];
      yield return
      [
        new HasNotRonList { values = NotRonList.From(["Hello", "World"])! },
        "HasNotRonList(values:NotRonList(value:\"Hello\",next:NotRonList(value:\"World\",next:None)))",
      ];
      yield return [new VectorTuple(1, 2, 3), "VectorTuple(1f32,2f32,3f32)"];
      yield return
      [
        new DoesNotHaveProxyType { value = new Vector3(1, 2, 3) },
        "DoesNotHaveProxyType(value:\"(1, 2, 3)\")",
      ];
    }
  }

  [Theory]
  [TestCaseSource(nameof(ObjectSerializerTestData))]
  public void SerializesObjectCorrectly(object o, string expected)
  {
    var actual = Ron.Serialize(o);
    // Round Trip
    var parsed = Ron.Deserialize(actual, o.GetType());
    Assert.That(actual, Is.EqualTo(expected));
    o.ShouldDeepEqual(parsed);
  }
}
