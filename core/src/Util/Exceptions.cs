namespace RonCS;

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

public static class ExceptionExtensions
{
  public static UnknownEnumException<T> AsEnumException<T>(this T value)
    where T : Enum
  {
    return UnknownEnumException.Create(value);
  }
}
