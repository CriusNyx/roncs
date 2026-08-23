using RonCS;
using RonCS.Exceptions;

namespace RonTests;

public class LexErrorTests
{
  public static IEnumerable<object[]> InvalidInputReturnsCorrectError_Data
  {
    get
    {
      yield return ["\""];
      yield return ["?"];
      yield return ["\\"];
    }
  }

  [Theory]
  [TestCaseSource(nameof(InvalidInputReturnsCorrectError_Data))]
  public void InvalidInputReturnsCorrectError(string input)
  {
    var parsed = Ron.ParseResult(input);

    Assert.That(parsed.IsErr());
    Assert.That(parsed.UnwrapErr(), Is.TypeOf<LexerException>());
    Assert.Throws<LexerException>(() => Ron.Parse(input));
  }
}
