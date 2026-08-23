using CriusNyx.Util;

namespace RonCS.Exceptions;

public static class RonException
{
  public static NotImplementedException CreateNotImplemented(
    string methodName,
    params object[] args
  )
  {
    return new NotImplementedException(
      $"{methodName} is not implemented for {args.Select(x => x.ToString())!.StringJoin(", ")}"
    );
  }
}
