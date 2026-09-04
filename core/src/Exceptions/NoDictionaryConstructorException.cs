namespace RonCS.Exceptions;

/// <summary>
/// Indicates that the type could not be initialized because it does not have a constructor with a dictionary type argument.
/// </summary>
/// <param name="type"></param>
public class NoDictionaryConstructorException(Type type) : Exception
{
  /// <inheritdoc/>
  public override string Message =>
    $"Expected {type} to have a constructor that accepts a dictionary, or a set of key value pairs, but it does not.";
}
