namespace RonCS;

public class NoEmptyConstructorException(Type type) : Exception
{
  public override string Message =>
    $"Expected {type} to have a parameterless constructor, but it does not.";
}
