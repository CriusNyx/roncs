using CriusNyx.Util;

namespace RonCS.AST;

/// <summary>
/// Base class for struct AST elements.
/// </summary>
/// <param name="name"></param>
[DebugPrint]
[Serializable]
public abstract class RonStruct(RonElement? name) : RonElement
{
  [DebugField]
  public RonElement? Name { get; set; } = name;
}

/// <summary>
/// AST element for a ron Unit struct with no body.
/// </summary>
/// <param name="name"></param>
[DebugPrint]
[Serializable]
public class RonUnitStruct(RonElement? name) : RonStruct(name)
{
  public override string RonPrint(RonPrintOptions options)
  {
    return Name?.RonPrint(options) ?? "";
  }
}

/// <summary>
/// AST element for a struct with a tuple body.
/// </summary>
/// <param name="name"></param>
/// <param name="body"></param>
[DebugPrint]
[Serializable]
public class RonTupleStruct(RonElement? name, RonElement? body) : RonStruct(name)
{
  /// <summary>
  /// The body of the tuple. This should be a RonTuple.
  /// </summary>
  [DebugField]
  public RonElement? Body = body;

  public override string RonPrint(RonPrintOptions options)
  {
    return Name?.RonPrint(options) + Body?.RonPrint(options);
  }
}

/// <summary>
/// AST element for a ron struct with named values.
/// </summary>
/// <param name="name"></param>
/// <param name="body"></param>
[DebugPrint]
[Serializable]
public class RonNamedValueStruct(RonElement? name, params RonElement[]? body) : RonStruct(name)
{
  /// <summary>
  /// The body of the named value struct. These should be RonNamedValues.
  /// </summary>
  [DebugField]
  public RonElement[]? Body = body;

  public override string RonPrint(RonPrintOptions options)
  {
    switch (options.mode)
    {
      case RonPrintMode.Compact:

        return Name?.RonPrint(options)
          + "("
          + Body?.Select(x => x.RonPrint(options)).StringJoin(",")
          + ")";
      case RonPrintMode.Pretty:
        return Name?.RonPrint(options)
          + "(\n"
          + Body?.Select(x => x.RonPrint(options)).StringJoin(",\n").Indent(options.indent)
          + "\n)";
      default:
        throw options.mode.AsEnumException();
    }
  }
}

/// <summary>
/// AST element for a ron map struct.
/// </summary>
/// <param name="name"></param>
/// <param name="mapBody"></param>
[DebugPrint]
[Serializable]
public class RonMapStruct(RonElement? name, RonElement? mapBody) : RonStruct(name)
{
  /// <summary>
  /// Body of the struct map.
  /// </summary>
  [DebugField]
  public RonElement? MapBody => mapBody;

  public override string RonPrint(RonPrintOptions options)
  {
    return Name?.RonPrint(options) + MapBody?.RonPrint(options);
  }
}
