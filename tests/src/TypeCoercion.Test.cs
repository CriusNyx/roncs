using DeepEqual.Syntax;
using RonCS;

namespace RonTests;

public class HasFromAttr
{
  public string value = null!;

  [RonFrom]
  public static HasFromAttr From(ForFromAttr source)
  {
    return new HasFromAttr { value = source.value };
  }
}

public class ForFromAttr
{
  public string value = null!;
}

public class ForIntoAttr
{
  public string value = null!;
}

public class HasIntoAttr
{
  public string value = null!;

  [RonInto]
  public ForIntoAttr Into()
  {
    return new ForIntoAttr { value = value };
  }
}

public class TypeCoercionTests
{
  [SetUp]
  public void Setup()
  {
    Ron.RegisterTypes(
      typeof(HasFromAttr),
      typeof(ForFromAttr),
      typeof(ForIntoAttr),
      typeof(HasIntoAttr)
    );
  }

  public static IEnumerable<object[]> TypeCoercionTestCases
  {
    get
    {
      yield return
      [
        "ForFromAttr(value: \"foo\")",
        typeof(HasFromAttr),
        new HasFromAttr { value = "foo" },
      ];
      yield return
      [
        "HasIntoAttr(value: \"foo\")",
        typeof(ForIntoAttr),
        new ForIntoAttr { value = "foo" },
      ];
    }
  }

  [Theory]
  [TestCaseSource(nameof(TypeCoercionTestCases))]
  public void CanPerformTypeCoercion(string ronSource, Type targetType, object expected)
  {
    var actual = Ron.Deserialize(ronSource, targetType);
    actual.ShouldDeepEqual(expected);
    Assert.That(actual.GetType(), Is.EqualTo(expected.GetType()));
  }
}
