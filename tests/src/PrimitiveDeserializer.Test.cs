using RonCS;

namespace RonTests;

public class PrimitiveDeserializerTests
{
  [Theory]
  [TestCase(["\"\"", ""])]
  [TestCase(["\"Hello World\"", "Hello World"])]
  [TestCase(["\"\\r\\n\"", "\r\n"])]
  [TestCase(["\"\\0\"", "\0"])]
  [TestCase(["\"\\r\"", "\r"])]
  [TestCase(["\"\\n\"", "\n"])]
  [TestCase(["\"\\\"\"", "\""])]
  [TestCase(["r#\"\\n\"#", "\\n"])]
  [TestCase(["\"\\u{1F339}\"", "🌹"])]
  public void StringDeserializesCorrectly(string source, string expected)
  {
    var actual = Ron.Deserialize(source, typeof(string));
    Assert.That(actual, Is.EqualTo(expected));
  }

  [Theory]
  // byte
  [TestCase("0b1u8", (byte)0b1)]
  [TestCase("0b10u8", (byte)0b10)]
  [TestCase("0o10u8", (byte)8)]
  [TestCase("10u8", (byte)10)]
  [TestCase("0x0u8", (byte)0x0)]
  [TestCase("0xAu8", (byte)0xA)]
  [TestCase("0xABu8", (byte)0xAB)]
  // char byte
  [TestCase("b'a'", (byte)'a')]
  [TestCase("b'\\\\'", (byte)'\\')]
  [TestCase("b'\\xFF'", (byte)0xff)]
  // sbyte
  [TestCase("-0b1i8", (sbyte)-0b1)]
  [TestCase("-0b10i8", (sbyte)-0b10)]
  [TestCase("-0o10i8", (sbyte)-8)]
  [TestCase("-10i8", (sbyte)-10)]
  [TestCase("-0x0i8", (sbyte)-0x0)]
  [TestCase("-0xAi8", (sbyte)-0xA)]
  [TestCase("-0x7Bi8", (sbyte)-0x7B)]
  // ushort
  [TestCase("0b1101_1101_1101_1101u16", (ushort)0b1101_1101_1101_1101)]
  [TestCase("0o123_123u16", (ushort)0xA653)]
  [TestCase("1234u16", (ushort)1234)]
  [TestCase("0x1234u16", (ushort)0x1234)]
  // short
  [TestCase("-0b0101_1101_1101_1101i16", (short)-0b0101_1101_1101_1101)]
  [TestCase("-0o12312i16", (short)-5322)]
  [TestCase("-1234i16", (short)-1234)]
  [TestCase("-0x1234i16", (short)-0x1234)]
  // uint
  [TestCase(
    "0b11011101_11011101_11011101_11011101u32",
    (uint)0b11011101_11011101_11011101_11011101
  )]
  [TestCase("0o123_123_123u32", (uint)0x14CA653)]
  [TestCase("12345678u32", (uint)12345678)]
  [TestCase("0xFF_FF_FF_FFu32", (uint)0xFFFFFFFF)]
  // int
  [TestCase(
    "-0b01011101_11011101_11011101_11011101i32",
    (int)-0b01011101_11011101_11011101_11011101
  )]
  [TestCase("-0o123_123_123i32", (int)-0x14CA653)]
  [TestCase("-12345678i32", (int)-12345678)]
  [TestCase("-0x7F_FF_FF_FFi32", (int)-0x7FFFFFFF)]
  // ulong
  [TestCase(
    "0b10000000_10000000_10000000_10000000_10000000_10000000_10000000_10000000u64",
    (ulong)0x80_80_80_80_80_80_80_80
  )]
  [TestCase("0o1234567u64", (ulong)342391)]
  [TestCase("1234567u64", (ulong)1234567)]
  [TestCase("0xFFFF_FFFF_FFFF_FFFFu64", (ulong)0xFFFF_FFFF_FFFF_FFFF)]
  // long
  [TestCase(
    "-0b01000000_10000000_10000000_10000000_10000000_10000000_10000000_10000000i64",
    (long)-0x40_80_80_80_80_80_80_80
  )]
  [TestCase("-0o1234567i64", (long)-342391)]
  [TestCase("-1234567i64", (long)-1234567)]
  [TestCase("-0x7FFF_FFFF_FFFF_FFFFi64", (long)-0x7FFF_FFFF_FFFF_FFFF)]
  public void IntegerDeserializesCorrectly(string source, object expected)
  {
    var actual = Ron.Deserialize(source, null);
    Assert.That(actual, Is.EqualTo(expected));
    Assert.That(actual.GetType() == expected.GetType());
  }

  [Theory]
  // Float
  [TestCase("1f32", (float)1)]
  [TestCase("1.0f32", (float)1)]
  [TestCase("1.1f32", (float)1.1)]
  [TestCase("1.10000001f32", (float)1.10000001)]
  [TestCase("0.1f32", (float)0.1)]
  [TestCase("1e32f32", (float)1e32)]
  [TestCase("1e-32f32", (float)1e-32)]
  // Double
  [TestCase("1f64", (double)1)]
  [TestCase("1.0f64", (double)1)]
  [TestCase("1.1f64", (double)1.1)]
  [TestCase("1.10000001f64", (double)1.10000001)]
  [TestCase("0.1f64", (double)0.1)]
  [TestCase("1e32f64", (double)1e32)]
  [TestCase("1e-32f64", (double)1e-32)]
  public void FloatDeserializesCorrectly(string source, object expected)
  {
    var actual = Ron.Deserialize(source, null);
    Assert.That(actual, Is.EqualTo(expected));
    Assert.That(actual.GetType() == expected.GetType());
  }
}
