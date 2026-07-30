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
public class RonUnitStruct(RonElement? name) : RonStruct(name) { }

[DebugPrint]
[Serializable]
public class RonTupleStruct(RonElement? name, RonElement? body) : RonStruct(name)
{
  [DebugField]
  public RonElement? Body = body;
}

[DebugPrint]
[Serializable]
public class RonNamedValueStruct(RonElement? name, RonElement[]? values) : RonStruct(name)
{
  [DebugField]
  public RonElement[]? Values = values;
}

[DebugPrint]
[Serializable]
public class RonMapStruct(RonElement? name, RonElement? mapBody) : RonStruct(name)
{
  [DebugField]
  public RonElement? MapBody => mapBody;
}
