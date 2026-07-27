namespace Ron;

public abstract class ByteKind;

public class RegularByteKind : ByteKind;

public class RawByteKind : ByteKind
{
  public byte? hashCount;
}

public class BytesExpr : Expr
{
  public RonSpan? span;
  public string? raw;
  public IEnumerable<byte>? value;
}
