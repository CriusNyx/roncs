using RonCS;

namespace RonTests;

public class PrimitiveSerializerTests
{
  public static IEnumerable<object[]> DecimalTestData
  {
    get
    {
      yield return [0.0m, "0"];
      yield return [0.1m, "0.1"];
      yield return [0.3m, "0.3"];
      yield return [1m, "1"];
      yield return [1.1m, "1.1"];
      yield return [-1m, "-1"];
      yield return [-0.3m, "-0.3"];
      yield return [-1.3m, "-1.3"];
    }
  }

  [Theory]
  // Boolean
  [TestCase(false, "false")]
  [TestCase(true, "true")]
  // Char
  [TestCase('a', "\'a\'")]
  [TestCase('\\', "'\\\\'")]
  [TestCase('\'', "'\\''")]
  // Integers
  [TestCase((byte)0, "0u8")]
  [TestCase((sbyte)-1, "-1i8")]
  [TestCase((short)-1, "-1i16")]
  [TestCase((ushort)0, "0u16")]
  [TestCase((int)-1, "-1i32")]
  [TestCase((uint)0, "0u32")]
  [TestCase((long)-1, "-1i64")]
  [TestCase((ulong)0, "0u64")]
  // Special Floats
  [TestCase(float.PositiveInfinity, "inf")]
  [TestCase(float.NegativeInfinity, "-inf")]
  [TestCase(float.NaN, "NaN")]
  [TestCase(double.PositiveInfinity, "inf")]
  [TestCase(double.NegativeInfinity, "-inf")]
  [TestCase(double.NaN, "NaN")]
  // Regular Floats
  [TestCase(0f, "0f32")]
  [TestCase(1f, "1f32")]
  [TestCase(-1f, "-1f32")]
  [TestCase(0.1f, "0.1f32")]
  [TestCase(1e12f, "1e12f32")]
  [TestCase(1e-12f, "1e-12f32")]
  [TestCase(-1e12f, "-1e12f32")]
  [TestCase(-1e-12f, "-1e-12f32")]
  [TestCase(0.0, "0f64")]
  [TestCase(1.0, "1f64")]
  [TestCase(-1.0, "-1f64")]
  [TestCase(0.1, "0.1f64")]
  [TestCase(1e12, "1e12f64")]
  [TestCase(1e-12, "1e-12f64")]
  [TestCase(-1e12, "-1e12f64")]
  [TestCase(-1e-12, "-1e-12f64")]
  [TestCaseSource(nameof(DecimalTestData))]
  public void CanSerializePrimitives(object value, string expected)
  {
    var actual = Ron.Serialize(value);
    Assert.That(actual, Is.EqualTo(expected));
    // Roundtrip Test
    var parsed = Ron.Deserialize(actual, value.GetType());
    Assert.That(parsed, Is.EqualTo(value));
  }
}
