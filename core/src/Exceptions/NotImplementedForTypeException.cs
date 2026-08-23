namespace RonCS.Exceptions;

public class NotImplementedForArgumentTypeException(string methodName, Type type) : Exception
{
  public override string Message => $"{methodName} is not implemented for type ${type}";
}
