using CriusNyx.Util;
using RonCS;

namespace RonTests;

public class DeserializerErrorTests
{
  public static IEnumerable<object[]> DeserializingInvalidObjectCausesError_Data
  {
    get
    {
      yield return ["(baz:\"Hello\")", typeof(SimpleClass), ":.baz"];
      yield return ["SimpleClass(baz:\"Hello\")", typeof(SimpleClass), ":SimpleClass.baz"];
      yield return ["[SimpleClass(baz:\"Hello\")]", typeof(SimpleClass[]), "[0]:SimpleClass.baz"];
      yield return
      [
        "[None, SimpleClass(baz:\"Hello\")]",
        typeof(SimpleClass[]),
        "[1]:SimpleClass.baz",
      ];
      yield return
      [
        "{\"value\": SimpleClass(baz:\"Hello\")}",
        typeof(Dictionary<string, SimpleClass>),
        "[\"value\"]:SimpleClass.baz",
      ];
      yield return
      [
        "[[SimpleClass(baz:\"Hello\")]]",
        typeof(SimpleClass[][]),
        "[0][0]:SimpleClass.baz",
      ];
    }
  }

  [Theory]
  [TestCaseSource(nameof(DeserializingInvalidObjectCausesError_Data))]
  public void DeserializingInvalidObjectCausesError(
    string ronString,
    Type ronType,
    string expectedErrorPath
  )
  {
    var result = Ron.DeserializeResult(ronString, ronType);
    Assert.That(result.IsErr());
    var exception = result.UnwrapErr().AsNotNull<DeserializationException>();
    var reasons = exception.Reasons;
    var ronPath = reasons.First().ronPath;
    Assert.That(ronPath, Is.EqualTo(expectedErrorPath));
  }
}
