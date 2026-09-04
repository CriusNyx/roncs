namespace RonCS.Exceptions;

/// <summary>
/// Deserialized object could not be constructed because there is no parameterless constructor.
/// </summary>
/// <param name="type"></param>
public class NoEmptyConstructorException(Type type) : Exception
{
  /// <inheritdoc/>
  public override string Message =>
    $"Expected {type} to have a parameterless constructor, but it does not.";
}
