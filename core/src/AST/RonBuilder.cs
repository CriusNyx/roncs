namespace RonCS.AST;

/// <summary>
/// Builder for RON AST's
/// </summary>
public static class RonBuilder
{
  /// <summary>
  /// Create a some value
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static RonSome Some(RonElement value)
  {
    return new(value);
  }

  /// <summary>
  /// Create a tuple with the specified child values
  /// </summary>
  /// <param name="values"></param>
  /// <returns></returns>
  public static RonTuple Tuple(params RonElement[] values)
  {
    return new RonTuple(values);
  }

  /// <summary>
  /// Create an identifier with the specified name
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  public static RonIdentifier Ident(string name)
  {
    return new RonIdentifier(name);
  }

  /// <summary>
  /// Create a raw identifier with the specified name
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  public static RonRawIdentifier RawIdent(string name)
  {
    return new RonRawIdentifier(name);
  }

  /// <summary>
  /// Create a name value with a specified name and value.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static RonNamedValue NamedValue(RonElement name, RonElement value)
  {
    return new RonNamedValue(name, value);
  }

  /// <summary>
  /// Create a named value with the specified name and value.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static RonNamedValue NamedValue(string name, RonElement value)
  {
    return new RonNamedValue(Ident(name), value);
  }

  /// <summary>
  /// Create a struct with a name and no body.
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  public static RonUnitStruct UnitStruct(string name)
  {
    return new RonUnitStruct(Ident(name));
  }

  /// <summary>
  /// Create a ron boolean with the specified value.
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static RonBool Bool(bool value)
  {
    return new RonBool(value);
  }

  /// <summary>
  /// Create a tuple struct with the specified name and elements.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="body"></param>
  /// <returns></returns>
  public static RonTupleStruct TupleStruct(string name, params RonElement[] body)
  {
    return new RonTupleStruct(Ident(name), Tuple(body));
  }

  /// <summary>
  /// Create a named value struct with the specified name, and body.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="body"></param>
  /// <returns></returns>
  public static RonNamedValueStruct NamedValueStruct(string name, params RonElement[] body)
  {
    return new RonNamedValueStruct(Ident(name), body);
  }

  /// <summary>
  /// Create a map item with the specified key and value.
  /// Typically the key should be a string.
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static RonMapItem MapItem(RonElement key, RonElement value)
  {
    return new RonMapItem(key, value);
  }

  /// <summary>
  /// Create a map with the specified values.
  /// </summary>
  /// <param name="values"></param>
  /// <returns></returns>
  public static RonMap Map(params RonElement[] values)
  {
    return new RonMap(values);
  }

  /// <summary>
  /// Create a list with the specified values.
  /// </summary>
  /// <param name="values"></param>
  /// <returns></returns>
  public static RonList List(params RonElement[] values)
  {
    return new RonList(values);
  }

  /// <summary>
  /// Create a range with the specified values.
  /// </summary>
  /// <param name="left"></param>
  /// <param name="op"></param>
  /// <param name="right"></param>
  /// <returns></returns>
  public static RonRange Range(RonElement? left, RonRangeOperator op, RonElement? right)
  {
    return new RonRange(left, op, right);
  }

  /// <summary>
  /// Create a document with the specified element.
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static RonDocument Ron(RonElement value)
  {
    return new RonDocument(value);
  }

  /// <summary>
  /// A Ron None.
  /// </summary>
  public static RonNone None => new();
}
