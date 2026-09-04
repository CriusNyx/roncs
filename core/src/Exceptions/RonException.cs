namespace RonCS.Exceptions;

/// <summary>
/// Base class for ron exceptions.
/// </summary>
public static class RonException
{
  internal static NotImplementedException CreateNotImplemented(
    string methodName,
    params object[] args
  )
  {
    return new NotImplementedException(
      $"{methodName} is not implemented for {args.Select(x => x.ToString())!.StringJoin(", ")}"
    );
  }
}
