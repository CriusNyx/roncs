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
    Assert.False(Ron.TryParse(input, out var doc, out var exception));

    Assert.That(exception, Is.TypeOf<LexerException>());
    Assert.Throws<LexerException>(() => Ron.Parse(input));
  }
}
