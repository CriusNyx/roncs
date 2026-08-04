using CriusNyx.Results;
using CriusNyx.Util;

[DebugPrint]
[Serializable]
public abstract class RonStruct(RonElement? name) : RonElement
{
  [DebugField]
  public RonElement? Name { get; set; } = name;
}

[DebugPrint]
[Serializable]
public class RonUnitStruct(RonElement? name) : RonStruct(name)
{
  public override string RonPrint(RonPrintOptions options)
  {
    return Name?.RonPrint(options) ?? "";
  }
}

[DebugPrint]
[Serializable]
public class RonTupleStruct(RonElement? name, RonElement? body) : RonStruct(name)
{
  [DebugField]
  public RonElement? Body = body;

  public override string RonPrint(RonPrintOptions options)
  {
    return Name?.RonPrint(options) + Body?.RonPrint(options);
  }
}

[DebugPrint]
[Serializable]
public class RonNamedValueStruct(RonElement? name, params RonElement[]? values) : RonStruct(name)
{
  [DebugField]
  public RonElement[]? Values = values;

  public override string RonPrint(RonPrintOptions options)
  {
    switch (options.mode)
    {
      case RonPrintMode.Compact:

        return Name?.RonPrint(options)
          + "("
          + Values?.Select(x => x.RonPrint(options)).StringJoin(",")
          + ")";
      case RonPrintMode.Pretty:
        return Name?.RonPrint(options)
          + "(\n"
          + Values?.Select(x => x.RonPrint(options)).StringJoin(",\n").Indent(options.indent)
          + "\n)";
      default:
        throw options.mode.AsEnumException();
    }
  }
}

[DebugPrint]
[Serializable]
public class RonMapStruct(RonElement? name, RonElement? mapBody) : RonStruct(name)
{
  [DebugField]
  public RonElement? MapBody => mapBody;

  public override string RonPrint(RonPrintOptions options)
  {
    return Name?.RonPrint(options) + MapBody?.RonPrint(options);
  }
}
