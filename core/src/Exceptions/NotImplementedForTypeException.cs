namespace RonCS.Exceptions;

/// <summary>
/// Could not call the specified method because it does not exist on the type.
/// </summary>
/// <param name="methodName"></param>
/// <param name="type"></param>
public class NotImplementedForArgumentTypeException(string methodName, Type type) : Exception
{
  public override string Message => $"{methodName} is not implemented for type ${type}";
}
