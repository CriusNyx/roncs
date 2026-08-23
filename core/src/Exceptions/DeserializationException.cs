using CriusNyx.Util;

namespace RonCS.Exceptions;

public class DeserializationException(params DeserializationExceptionReason[] reasons) : Exception
{
  public readonly IEnumerable<DeserializationExceptionReason> Reasons = reasons.ToArray();

  public static DeserializationException FromOthers(IEnumerable<Exception> others)
  {
    return others
      .SelectMany(x => x.AsNotNull<DeserializationException>().Reasons)
      .Transform(reasons => new DeserializationException(reasons.ToArray()));
  }
}

public abstract class DeserializationExceptionReason(string ronPath)
{
  public readonly string ronPath = ronPath;

  public abstract string Message { get; }
}

public class NoFieldOrPropertyException(string ronPath, Type deserializedType, string fieldName)
  : DeserializationExceptionReason(ronPath)
{
  public override string Message =>
    $"Ron element at {ronPath} could not be assigned to {deserializedType}.{fieldName} because {deserializedType} does not have the specified field or property.";
}
