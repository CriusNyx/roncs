public static class RonBuilder
{
  public static RonSome Some(RonElement value)
  {
    return new(value);
  }

  public static RonTuple Tuple(params RonElement[] values)
  {
    return new RonTuple(values);
  }

  public static RonIdentifier Ident(string value)
  {
    return new RonIdentifier(value);
  }

  public static RonRawIdentifier RawIdent(string value)
  {
    return new RonRawIdentifier(value);
  }

  public static RonNamedValue NamedValue(RonElement name, RonElement value)
  {
    return new RonNamedValue(name, value);
  }

  public static RonNamedValue NamedValue(string name, RonElement value)
  {
    return new RonNamedValue(Ident(name), value);
  }

  public static RonUnitStruct UnitStruct(string name)
  {
    return new RonUnitStruct(Ident(name));
  }

  public static RonBool Bool(bool value)
  {
    return new RonBool(value);
  }

  public static RonTupleStruct TupleStruct(string name, params RonElement[] body)
  {
    return new RonTupleStruct(Ident(name), Tuple(body));
  }

  public static RonNamedValueStruct NamedValueStruct(string name, params RonElement[] body)
  {
    return new RonNamedValueStruct(Ident(name), body);
  }

  public static RonMapItem MapItem(RonElement key, RonElement value)
  {
    return new RonMapItem(key, value);
  }

  public static RonMap Map(params RonElement[] values)
  {
    return new RonMap(values);
  }

  public static RonList List(params RonElement[] values)
  {
    return new RonList(values);
  }

  public static RonRange Range(RonElement? left, RonRangeOperator op, RonElement? right)
  {
    return new RonRange(left, op, right);
  }

  public static RonDocument Ron(RonElement value)
  {
    return new RonDocument(value);
  }

  public static RonNone None => new();
}
