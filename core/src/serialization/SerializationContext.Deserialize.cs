using System.Collections;
using System.Reflection;
using CriusNyx.Util;

namespace RonCS;

public partial class SerializationContext(SerializationContext? parentContext = null)
{
  public readonly HashSet<Type> RonTypes = new();
  private SerializationContext? parentContext = parentContext;

  /// <summary>
  /// Register a new known type
  /// </summary>
  /// <param name="type"></param>
  public void RegisterType(Type type)
  {
    RonTypes.Add(type);
  }

  /// <summary>
  /// Deserialize a ron element
  /// </summary>
  /// <param name="element"></param>
  /// <param name="typeHint"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public object DeserializeElement(RonElement? element, Type? typeHint)
  {
    var output = element switch
    {
      RonDocument doc => DeserializeElement(doc.Value!, typeHint),
      StringValue str => str.Evaluate(),
      NumberValue numValue => numValue.EvaluateNumber(typeHint),
      RonBool boolVal => boolVal.Value,
      RonChar charVal => charVal.Value,
      RonSome some => DeserializeElement(some.value, typeHint),
      RonNone => null!,
      RonUnitStruct unitStruct => DeserializeUnitClass(typeHint.NotNull("typeHint"), unitStruct),
      RonNamedValueStruct ronNamedValueStruct => DeserializeNamedValueClass(
        typeHint,
        ronNamedValueStruct
      ),
      RonTupleStruct tupleStruct => DeserializeTupleStructClass(typeHint, tupleStruct),
      RonMapStruct mapStruct => DeserializeElement(
        mapStruct.MapBody,
        GetTypeFromName(mapStruct.Name.IdentifierName(), typeHint)
      ),
      RonTuple tuple => DeserializeTupleClassBody(typeHint.NotNull("typeHint"), tuple),
      RonList list => DeserializeList(typeHint.NotNull("typeHint"), list),
      RonMap map => DeserializeMap(map, typeHint),
      _ => throw new NotImplementedException(),
    };
    return ConvertType(output, typeHint)!;
  }

  /// <summary>
  ///  Deserialize Unit Tuple
  /// </summary>
  /// <param name="typeHint"></param>
  /// <param name="unitStruct"></param>
  /// <returns></returns>
  public object DeserializeUnitClass(Type typeHint, RonUnitStruct unitStruct)
  {
    if (typeHint.IsEnum)
    {
      return DeserializeEnum(typeHint, unitStruct.Name.NotNull(nameof(unitStruct.Name)));
    }

    var actualType = GetTypeFromName(unitStruct.Name.IdentifierName(), typeHint)
      .NotNull("actualType");

    var constructor = actualType.GetConstructor([]);
    var output = constructor?.Invoke([]);

    return output!;
  }

  /// <summary>
  ///  Deserialize Enum
  /// </summary>
  /// <param name="enumType"></param>
  /// <param name="identifier"></param>
  /// <returns></returns>
  public object DeserializeEnum(Type enumType, RonElement identifier)
  {
    return Enum.Parse(enumType, identifier.IdentifierName().NotNull(nameof(identifier)));
  }

  /// <summary>
  /// Deserialize name value struct
  /// </summary>
  /// <param name="typeHint"></param>
  /// <param name="element"></param>
  /// <returns></returns>
  public object DeserializeNamedValueClass(Type? typeHint, RonNamedValueStruct element)
  {
    var actualType = GetTypeFromName(element.Name.IdentifierName(), typeHint).NotNull("actualType");
    var constructor = actualType.GetConstructor([]);
    var output = constructor?.Invoke([]);

    foreach (var field in element.Values.NotNull("Values"))
    {
      DeserializeClassField(field, output.NotNull());
    }

    return output!;
  }

  /// <summary>
  /// Deserialize named field.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="instance"></param>
  /// <returns></returns>
  public object DeserializeClassField(RonElement element, object instance)
  {
    Type type = instance.GetType();
    if (element is RonNamedValue namedValue)
    {
      var fieldName = namedValue.name.IdentifierName().NotNull("fieldName");
      var fieldInfo = type.GetField(fieldName).NotNull("fieldInfo");
      var fieldValue = DeserializeElement(
        namedValue.value.NotNull("namedValue.value"),
        fieldInfo.FieldType
      );
      fieldInfo.SetValue(instance, fieldValue);
    }
    return instance;
  }

  /// <summary>
  ///  Deserialize Tuple struct
  /// </summary>
  /// <param name="typeHint"></param>
  /// <param name="element"></param>
  /// <returns></returns>
  public object DeserializeTupleStructClass(Type? typeHint, RonTupleStruct element)
  {
    var body = element.Body.AsNotNull<RonTuple>();

    var actualType = GetTypeFromName(element.Name.IdentifierName(), typeHint).NotNull("actualType");
    return DeserializeTupleClassBody(actualType, body);
  }

  /// <summary>
  /// Deserialize ron tuple class body.
  /// </summary>
  /// <param name="typeHint"></param>
  /// <param name="body"></param>
  /// <returns></returns>
  public object DeserializeTupleClassBody(Type typeHint, RonTuple body)
  {
    // the argument types.
    Type?[] inferredArgumentTypes = body.Values.NotNull().Select(InferType).ToArray();
    var constructor = typeHint
      .GetConstructors()
      .Match(inferredArgumentTypes)
      .NotNull("constructor");

    // Find a constructor which can probably be executed with the provided arguments.
    var argsWithTypes = constructor
      .GetParameters()
      .Select(x => x.ParameterType)
      .OuterZip(body.Values.NotNull("body.Values"));

    // Deserialize the arguments using the constructor parameters.
    var args = argsWithTypes
      .Where(x => x.left != null)
      .Select(x => DeserializeElement(x.right, x.left))
      .ToArray();

    // Invoke the constructor with the parameters.
    return constructor.Invoke(args);
  }

  /// <summary>
  /// Deserialize list
  /// </summary>
  /// <param name="typeHint"></param>
  /// <param name="list"></param>
  /// <returns></returns>
  public object DeserializeList(Type? typeHint, RonList list)
  {
    // Null checking
    typeHint = typeHint.NotNull(nameof(typeHint));

    // List element type.
    var listElementType = typeHint.GetListType().NotNull("typeHint.GetListType()")!;

    // Deserialize list elements.
    var elements = list
      .Values.NotNull(nameof(list.Values))
      .Select(x => DeserializeElement(x, listElementType));

    // Create the output array.
    var array = Array.CreateInstance(listElementType, elements.Count());
    foreach (var (element, index) in elements.WithIndex())
    {
      array.SetValue(element, index);
    }

    // Attempt to initialize the type using an IEnumerable
    // This makes it possible to deserialize Lists and HashSets.
    if (
      typeHint.GetConstructor([typeof(IEnumerable<>).MakeGenericType(listElementType)])
      is ConstructorInfo cons
    )
    {
      return cons?.Invoke([array])!;
    }

    // Return the raw array otherwise.
    return array;
  }

  /// <summary>
  /// Deserialize map
  /// </summary>
  /// <param name="map"></param>
  /// <param name="typeHint"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public object DeserializeMap(RonMap map, Type? typeHint)
  {
    // Null check
    typeHint = typeHint.NotNull(nameof(typeHint));

    // Get the intermediate type for deserializing the map.
    var intermediateType = GetMapIntermediateType(typeHint).NotNull("GetMapType(typeHint)");

    // Get the value type for the intermediate dictionary.
    var valueType = intermediateType.GetDictionaryValueType().NotNull("valueType");

    // Get the type of the dictionary to generate.
    var dictType = typeof(IDictionary<,>).MakeGenericType([typeof(string), valueType]);

    // Create a new dictionary to store the values.
    IDictionary values = typeof(Dictionary<,>)
      .MakeGenericType([typeof(string), valueType])
      .Construct()
      .AsNotNull<IDictionary>("values");

    foreach (var element in map.Values.NotNull(nameof(map.Values)))
    {
      if (element is RonMapItem mapItem)
      {
        var key = mapItem.Key.AsNotNull<StringValue>(nameof(mapItem.Key)).Evaluate();
        var value = DeserializeElement(mapItem.Value, valueType);
        values.Add(key!, value);
      }
    }

    if (values.GetType() == typeHint || typeHint == dictType)
    {
      return values;
    }
    if (typeHint.GetConstructor([dictType]) is ConstructorInfo dictCons)
    {
      return dictCons.Invoke([values]);
    }
    if (
      typeHint.GetConstructor([
        typeof(IEnumerable<>).MakeGenericType([
          typeof(KeyValuePair<,>).MakeGenericType([typeof(string), valueType]),
        ]),
      ])
      is ConstructorInfo listCons
    )
    {
      return listCons.Invoke([values]);
    }
    throw new NotImplementedException();
  }

  /// <summary>
  /// Get a type from it's name.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="backupType"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public Type? GetTypeFromName(string? name, Type? backupType)
  {
    if (RonTypes.TryFind((x) => x.Name == name, out var result))
    {
      return result;
    }
    if (parentContext?.GetTypeFromName(name, backupType) is Type type)
    {
      return type;
    }
    if (name != null && name.ToLower() != backupType?.Name.ToLower())
    {
      throw new InvalidOperationException();
    }
    return backupType;
  }

  /// <summary>
  /// Get intermediate deserialization type, such as for a proxy.
  /// </summary>
  /// <param name="targetType"></param>
  /// <returns></returns>
  public Type GetIntermediateType(Type targetType)
  {
    return targetType;
  }

  /// <summary>
  /// Infer the type of the AST element.
  /// If the type cannot be inferred then return null.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public Type? InferType(RonElement? element)
  {
    return element switch
    {
      RonBool => typeof(bool),
      StringValue => typeof(string),
      NumberValue numVal => numVal.CSType(),
      RonDocument doc => InferType(doc.Value.NotNull("doc.value")),
      RonIdentifier ident => GetTypeFromName(ident.Value, null),
      RonRawIdentifier ident => GetTypeFromName(ident.value, null),
      RonSome some => InferType(some.value!),
      RonRange => typeof(Range),
      RonStruct ronStruct => InferType(ronStruct.Name),
      RonList => null,
      RonMap => null,
      RonMapItem => null,
      RonNamedValue => null,
      RonNone => null,
      RonTuple => null,
      null => null,
      _ => throw new NotImplementedException(),
    };
  }

  /// <summary>
  /// Get the value type for deserializing a ron map.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public Type? GetMapIntermediateType(Type? type)
  {
    if (type == null)
    {
      return null;
    }
    if (type.IsDictionaryType())
    {
      return type;
    }
    if (
      type.GetConstructors()
        .FirstOrDefault(x =>
          x.GetParameters() is [ParameterInfo info]
          && info.ParameterType.IsGenericType
          && info.ParameterType.GetGenericTypeDefinition() == typeof(IDictionary<,>)
        )
      is ConstructorInfo cons
    )
    {
      return cons.GetParameters().First().ParameterType;
    }
    return type;
  }

  /// <summary>
  /// Convert type from the source to the targetType.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="targetType"></param>
  /// <returns></returns>
  public object? ConvertType(object? source, Type? targetType)
  {
    if (targetType is null)
    {
      return source;
    }

    if (source is null)
    {
      return null;
    }
    var sourceType = source.GetType();

    if (sourceType.IsAssignableTo(targetType))
    {
      return source;
    }

    return TypeConversionCache.GetConverter(sourceType, targetType)(source);
  }

  /// <summary>
  /// Create global serialization context.
  /// </summary>
  /// <returns></returns>
  internal static SerializationContext CreateGlobalContext()
  {
    return new SerializationContext().Touch(context =>
    {
      context.RegisterType(typeof(object));
    });
  }
}
