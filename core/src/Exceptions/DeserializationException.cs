namespace RonCS.Exceptions;

/// <summary>
/// Ron document failed to deserialize.
/// </summary>
/// <param name="reasons"></param>
public class DeserializationException(params DeserializationExceptionReason[] reasons) : Exception
{
  /// <summary>
  /// The reasons why the document failed to deserialize.
  /// </summary>
  public readonly IEnumerable<DeserializationExceptionReason> Reasons = reasons.ToArray();

  /// <summary>
  /// TODO: What does this do?
  /// </summary>
  /// <param name="others"></param>
  /// <returns></returns>
  public static DeserializationException FromOthers(IEnumerable<Exception> others)
  {
    return others
      .SelectMany(x => x.AsNotNull<DeserializationException>().Reasons)
      .Transform(reasons => new DeserializationException(reasons.ToArray()));
  }
}

/// <summary>
/// Reason why a document failed to deserialize.
/// </summary>
/// <param name="ronPath"></param>
public abstract class DeserializationExceptionReason(string ronPath)
{
  /// <summary>
  /// The path of the element.
  /// </summary>
  public readonly string ronPath = ronPath;

  /// <summary>
  /// Reason message.
  /// </summary>
  public abstract string Message { get; }
}

/// <summary>
/// Indicates that a document failed to deserialize because the source type does not have the specified field.
/// </summary>
/// <param name="ronPath"></param>
/// <param name="deserializedType"></param>
/// <param name="fieldName"></param>
public class NoFieldOrPropertyException(string ronPath, Type deserializedType, string fieldName)
  : DeserializationExceptionReason(ronPath)
{
  /// <inheritdoc/>
  public override string Message =>
    $"Ron element at {ronPath} could not be assigned to {deserializedType}.{fieldName} because {deserializedType} does not have the specified field or property.";
}
