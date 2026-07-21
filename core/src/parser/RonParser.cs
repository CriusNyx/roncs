using CriusNyx.Util;
using Superpower;
using Superpower.Parsers;
using MultiParser = Superpower.TokenListParser<
  RonTokenKind,
  System.Collections.Generic.IEnumerable<RonElement>
>;
using TParser = Superpower.TokenListParser<RonTokenKind, RonElement>;

public static class RonParser
{
  /// <summary>
  /// Identifier = identifier | rawIdentifier;
  /// </summary>
  public static TParser Identifier = Parse.OneOf(
    Token
      .EqualTo(RonTokenKind.Identifier)
      .Select(x => new RonIdentifier(x.ToStringValue()) as RonElement),
    Token
      .EqualTo(RonTokenKind.RawIdentifier)
      .Select(x => new RonRawIdentifier(x.ToStringValue()) as RonElement)
  );

  // Value
  /// <summary>
  /// Value = Option | Tuple | Struct | Map | Primitive
  /// </summary>
  public static TParser Value = Parse.Ref(() =>
    Parse.OneOf(
      Option.NotNull("Option"),
      Tuple.NotNull("Tuple"),
      Struct.NotNull("Struct"),
      Primitive.NotNull("Primitive"),
      Map.NotNull("Map")
    )
  );

  // Options
  /// <summary>
  /// Some = 'Some' '(' Value ')';
  /// </summary>
  public static TParser Some = Token
    .EqualTo(RonTokenKind.Some)
    .IgnoreThen(Value.Parenthesized())
    .Select(value => new RonSome(value) as RonElement);

  /// <summary>
  /// None = 'None';
  /// </summary>
  public static TParser None = Token.EqualTo(RonTokenKind.None).Value(new RonNone() as RonElement);

  /// <summary>
  /// Option = Some | None;
  /// </summary>
  public static TParser Option = Parse.OneOf(Some, None);

  // Value Sets
  /// <summary>
  /// ValueSet = Value { Value ',' } [',']
  /// </summary>
  public static MultiParser ValueSet = Value
    .SeparatedBy(Token.EqualTo(RonTokenKind.Comma))
    .OptionalOrDefault([])
    .TrailingComma();

  /// <summary>
  /// NamedValue = Identifier ':' Value
  /// </summary>
  public static TParser NamedValue = Identifier
    .ThenIgnore(Token.EqualTo(RonTokenKind.Colon))
    .Then((ident) => Value.Select((value) => new RonNamedValue(ident, value) as RonElement));

  /// <summary>
  /// NamedValueSet = NamedValue { ',' NamedValue } [',']
  /// </summary>
  public static MultiParser NamedValueSet = NamedValue
    .SeparatedBy(Token.EqualTo(RonTokenKind.Comma))
    .OptionalOrDefault([])
    .TrailingComma();

  /// <summary>
  /// MapItem = Value ':' Value;
  /// </summary>
  public static TParser MapItem =
    from key in Value
    from colon in Token.EqualTo(RonTokenKind.Colon)
    from value in Value
    select new RonMapItem(key, value) as RonElement;

  /// <summary>
  /// MapItemSet = { MapItem } [','];
  /// </summary>
  public static MultiParser MapItemSet = MapItem
    .SeparatedBy(Token.EqualTo(RonTokenKind.Comma))
    .OptionalOrDefault([])
    .TrailingComma();

  /// <summary>
  /// RonMap
  /// </summary>
  public static TParser Map = MapItemSet
    .InCurly()
    .Select(values => new RonMap(values.ToArray()) as RonElement);

  public static TParser List = ValueSet
    .InSquare()
    .Select(values => new RonList(values.ToArray()) as RonElement);

  // Collections
  /// <summary>
  /// Tuple = '(' [ValueSet] ')';
  /// </summary>
  public static TParser Tuple =
    from body in ValueSet.Parenthesized()
    select new RonTuple(body.ToArray()) as RonElement;

  /// <summary>
  /// TupleStruct = Identifier Tuple;
  /// </summary>
  public static TParser TupleStruct =
    from name in Identifier
    from body in Tuple
    select new RonTupleStruct(name, body) as RonElement;

  /// <summary>
  /// NamedFieldStruct = Identifier '(' [NamedValueSet] ')';
  /// </summary>
  public static TParser NameFieldStruct =
    from name in Identifier
    from body in NamedValueSet.Parenthesized()
    select new RonNamedValueStruct(name, body.ToArray()) as RonElement;

  /// <summary>
  /// UnitStruct = Identifier;
  /// </summary>
  public static TParser UnitStruct = Identifier.Select(
    (name) => new RonUnitStruct(name) as RonElement
  );

  /// <summary>
  /// Struct = TupleStruct | NamedFieldStruct | UnitStruct;
  /// </summary>
  public static TParser Struct = Parse.OneOf(TupleStruct.Try(), NameFieldStruct.Try(), UnitStruct);

  // Primitive Values
  /// <summary>
  /// Boolean = 'true' | 'false';
  /// </summary>
  public static TParser Boolean = Parse.OneOf(
    Token.EqualTo(RonTokenKind.True).Value(new RonBool(true) as RonElement),
    Token.EqualTo(RonTokenKind.False).Value(new RonBool(false) as RonElement)
  );

  /// <summary>
  /// Number = Integer;
  /// </summary>
  public static TParser Number = Token
    .EqualTo(RonTokenKind.Number)
    .Select(x => NumberParser.Number_Parser.Parse(x.ToStringValue()) as RonElement);

  public static TokenListParser<RonTokenKind, RonRangeOperator> RangeOperatorExclusive = Token
    .EqualTo(RonTokenKind.RangeExclusive)
    .Value(RonRangeOperator.Exclusive);

  /// <summary>
  /// RangeOperator = '..' | '..=';
  /// </summary>
  public static TokenListParser<RonTokenKind, RonRangeOperator> RangeOperator = Parse.OneOf(
    RangeOperatorExclusive,
    Token.EqualTo(RonTokenKind.RangeInclusive).Value(RonRangeOperator.Inclusive)
  );

  /// <summary>
  /// RangeBinary = Number RangeOperator Number;
  /// </summary>
  public static TParser RangeBinary =
    from left in Number
    from op in RangeOperator
    from right in Number
    select new RonRange(left, op, right) as RonElement;

  /// <summary>
  /// RangeFrom = Number '..';
  /// </summary>
  public static TParser RangeFrom =
    from left in Number
    from op in RangeOperatorExclusive
    select new RonRange(left, op, null) as RonElement;

  /// <summary>
  /// RangeTo = RangeOperator Number;
  /// </summary>
  public static TParser RangeTo =
    from op in RangeOperator
    from right in Number
    select new RonRange(null, op, right) as RonElement;

  /// <summary>
  /// RangeFull = '..';
  /// </summary>
  public static TParser RangeFull = RangeOperatorExclusive.Select(
    (op) => new RonRange(null, op, null) as RonElement
  );

  // Range = RangeBinary | RangeFrom | RangeTo | RangeFull;
  public static TParser Range = Parse.OneOf(
    RangeBinary.Try(),
    RangeFrom.Try(),
    RangeTo.Try(),
    RangeFull.Try()
  );

  public static TParser String = Token
    .EqualTo(RonTokenKind.String)
    .Select(token => StringParser.String_Parser.Parse(token.ToStringValue()) as RonElement);

  /// <summary>
  /// Primitive = Boolean;
  /// </summary>
  // Range must appear first because we must check ranges before numbers.
  public static TParser Primitive = Parse.OneOf(Range.Try(), Number.Try(), String.Try(), Boolean);

  /// <summary>
  /// Ron = Extensions Value;
  /// </summary>
  public static TParser Ron = Value.Select(value => new RonDocument(value) as RonElement);
}

internal static class RonParserExtensions
{
  internal static TokenListParser<RonTokenKind, T> Parenthesized<T>(
    this TokenListParser<RonTokenKind, T> self
  )
  {
    return self.Between(
      Token.EqualTo(RonTokenKind.OpenParen),
      Token.EqualTo(RonTokenKind.ClosedParen)
    );
  }

  internal static TokenListParser<RonTokenKind, T> InCurly<T>(
    this TokenListParser<RonTokenKind, T> self
  )
  {
    return self.Between(
      Token.EqualTo(RonTokenKind.OpenCurly),
      Token.EqualTo(RonTokenKind.ClosedCurly)
    );
  }

  internal static TokenListParser<RonTokenKind, T> InSquare<T>(
    this TokenListParser<RonTokenKind, T> self
  )
  {
    return self.Between(
      Token.EqualTo(RonTokenKind.OpenSquare),
      Token.EqualTo(RonTokenKind.ClosedSquare)
    );
  }

  internal static TokenListParser<RonTokenKind, T> TrailingComma<T>(
    this TokenListParser<RonTokenKind, T> self
  )
  {
    return self.ThenIgnore(Token.EqualTo(RonTokenKind.Comma).Optional());
  }
}
