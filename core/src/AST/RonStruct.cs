using CriusNyx.Util;

[DebugPrint]
[Serializable]
public class RonUnitStruct(RonElement? name) : RonElement
{
  [DebugField]
  public RonElement? Name = name;
}

[DebugPrint]
[Serializable]
public class RonTupleStruct(RonElement? name, RonElement? body) : RonElement
{
  [DebugField]
  public RonElement? Name = name;

  [DebugField]
  public RonElement? Values = body;
}

[DebugPrint]
[Serializable]
public class RonNamedValueStruct(RonElement? name, RonElement[]? values) : RonElement
{
  [DebugField]
  public RonElement? Name = name;

  [DebugField]
  public RonElement[]? Values = values;
}
