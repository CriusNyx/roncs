namespace RonCS.Exceptions;

public class NoDictionaryConstructorException(Type type) : Exception
{
  public override string Message =>
    $"Expected {type} to have a constructor that accepts a dictionary, or a set of key value pairs, but it does not.";
}
