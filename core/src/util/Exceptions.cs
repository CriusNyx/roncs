public class UnknownEnumException<T>(T? value) : Exception
{
  public override string Message => $"Unexpected enum of type {value}";
}

public class UnknownEnumException
{
  public static UnknownEnumException<T> Create<T>(T? value)
  {
    return new UnknownEnumException<T>(value);
  }
}
