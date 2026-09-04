using System.Collections;
using System.Data;
using System.Reflection;
using RonCS.AST;
using RonCS.Exceptions;

namespace RonCS;

/// <summary>
/// Context for serializing ron elements.
/// Custom overrides for Ron are stored in the local or global serialization context.
/// </summary>
/// <param name="parentContext"></param>
public partial class SerializationContext(SerializationContext? parentContext = null)
{
  /// <summary>
  /// Set of types recognized by this serialization context.
  /// </summary>
  public readonly HashSet<Type> RonTypes = new();

  /// <summary>
  /// Proxy types recognized by this serialization context.
  /// </summary>
  public readonly Dictionary<Type, Type> proxyTypes = new Dictionary<Type, Type>();

  /// <summary>
  /// The parent serializtion context to inherrit.
  /// </summary>
  private SerializationContext? parentContext = parentContext;

  /// <summary>
  /// Deserialize a ron element
  /// </summary>
  /// <param name="element"></param>
  /// <param name="typeHint"></param>
  /// <param name="path"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  internal SerializationResult<object?> DeserializeElement(
    RonElement? element,
    Type? typeHint,
    string path
  )
  {
    SerializationResult<object?> Ok(object? value)
    {
      return SerializationResult<object?>.Ok(value!);
    }

    SerializationResult<object?> output = element switch
    {
      RonDocument doc => DeserializeElement(doc.Value!, typeHint, path),
      RonString str => Ok(str.Evaluate()),
      INumberValue numValue => Ok(numValue.EvaluateNumber(typeHint)),
      RonBool boolVal => Ok(boolVal.Value),
      RonChar charVal => Ok(charVal.Value),
      RonSome some => DeserializeElement(some.value, typeHint, path),
      RonNone => Ok(null),
      RonUnitStruct unitStruct => DeserializeUnitClass(typeHint.NotNull("typeHint"), unitStruct),
      RonNamedValueStruct ronNamedValueStruct => DeserializeNamedValueClass(
        typeHint,
        ronNamedValueStruct,
        path
      ),
      RonTupleStruct tupleStruct => DeserializeTupleStructClass(typeHint, tupleStruct, path),
      RonMapStruct mapStruct => DeserializeElement(
        mapStruct.MapBody,
        GetTypeFromName(mapStruct.Name.IdentifierName(), typeHint),
        path
      ),
      RonTuple tuple => DeserializeTupleClassBody(typeHint.NotNull("typeHint"), tuple, path),
      RonList list => DeserializeList(typeHint.NotNull("typeHint"), list, path),
      RonMap map => DeserializeMap(map, typeHint, path),
      _ => RonException.CreateNotImplemented(nameof(DeserializeElement)).AsErr<object?>(),
    };
    return output.Map(x => ConvertType(x, typeHint!))!;
  }

  /// <summary>
  ///  Deserialize Unit Tuple
  /// </summary>
  /// <param name="typeHint"></param>
  /// <param name="unitStruct"></param>
  /// <returns></returns>
  internal SerializationResult<object?> DeserializeUnitClass(
    Type typeHint,
    RonUnitStruct unitStruct
  )
  {
    if (typeHint.IsEnum)
    {
      return DeserializeEnum(typeHint, unitStruct.Name.NotNull(nameof(unitStruct.Name)));
    }

    var actualType = GetTypeFromName(unitStruct.Name.IdentifierName(), typeHint)
      .NotNull("actualType");

    var constructor = actualType.GetConstructor([]);
    if (constructor == null)
    {
      throw new NoEmptyConstructorException(typeHint);
    }
    var output = constructor?.Invoke([]);

    return output.AsOk();
  }

  /// <summary>
  ///  Deserialize Enum
  /// </summary>
  /// <param name="enumType"></param>
  /// <param name="identifier"></param>
  /// <returns></returns>
  internal SerializationResult<object?> DeserializeEnum(Type enumType, RonElement identifier)
  {
    return Enum.Parse(enumType, identifier.IdentifierName().NotNull(nameof(identifier))).AsOk()!;
  }

  /// <summary>
  /// Deserialize name value struct
  /// </summary>
  /// <param name="typeHint"></param>
  /// <param name="element"></param>
  /// <param name="path"></param>
  /// <returns></returns>
  internal SerializationResult<object?> DeserializeNamedValueClass(
    Type? typeHint,
    RonNamedValueStruct element,
    string path
  )
  {
    var typeName = element.Name.IdentifierName();
    var actualType = GetTypeFromName(typeName, typeHint).NotNull("actualType");
    var constructor = actualType.GetConstructor([]);
    if (constructor == null)
    {
      throw new NoEmptyConstructorException(actualType);
    }
    var instance = constructor.Invoke([]);

    // TODO: This seems wrong?
    foreach (var field in element.Body.NotNull("Values"))
    {
      var fieldResult = DeserializeClassField(field, instance.NotNull(), $"{path}:{typeName}");
      if (!fieldResult.isSuccess)
      {
        return fieldResult;
      }
    }

    return instance.AsOk()!;
  }

  /// <summary>
  /// Deserialize named field.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="instance"></param>
  /// <param name="path"></param>
  /// <returns></returns>
  internal SerializationResult<object?> DeserializeClassField(
    RonElement element,
    object instance,
    string path
  )
  {
    Type type = instance.GetType();
    if (element is RonNamedValue namedValue)
    {
      var fieldName = namedValue.name.IdentifierName().NotNull("fieldName");
      var fieldInfo = type.GetRonField(fieldName);
      if (fieldInfo == null)
      {
        return new DeserializationException(
          new NoFieldOrPropertyException($"{path}.{fieldName}", type, fieldName)
        ).AsErr<object?>();
      }
      var fieldValueResult = DeserializeElement(
        namedValue.value.NotNull("namedValue.value"),
        fieldInfo.MemberValueType(),
        $"{path}.{fieldName}"
      );

      // TODO: This seems maybe wrong.
      if (!fieldValueResult.isSuccess)
      {
        return fieldValueResult;
      }

      fieldInfo.AssignMember(instance, fieldValueResult.value!);
    }
    return instance.AsOk()!;
  }

  /// <summary>
  ///  Deserialize Tuple struct
  /// </summary>
  /// <param name="typeHint"></param>
  /// <param name="element"></param>
  /// <param name="path"></param>
  /// <returns></returns>
  internal SerializationResult<object?> DeserializeTupleStructClass(
    Type? typeHint,
    RonTupleStruct element,
    string path
  )
  {
    var body = element.Body.AsNotNull<RonTuple>();

    var actualType = GetTypeFromName(element.Name.IdentifierName(), typeHint).NotNull("actualType");
    return DeserializeTupleClassBody(actualType, body, path);
  }

  /// <summary>
  /// Deserialize ron tuple class body.
  /// </summary>
  /// <param name="typeHint"></param>
  /// <param name="body"></param>
  /// <param name="path"></param>
  /// <returns></returns>
  internal SerializationResult<object?> DeserializeTupleClassBody(
    Type typeHint,
    RonTuple body,
    string path
  )
  {
    // TODO: This should not throw. It should return an error result.
    // the argument types.
    Type?[] inferredArgumentTypes = body.Values.NotNull().Select(InferType).ToArray();
    var constructor = typeHint
      .GetConstructors()
      .MatchFirst(inferredArgumentTypes)
      .NotNull("constructor");

    // Find a constructor which can probably be executed with the provided arguments.
    var argsWithTypes = constructor
      .GetParameters()
      .Select(x => x.ParameterType)
      .OuterZip(body.Values.NotNull("body.Values"));

    // Deserialize the arguments using the constructor parameters.
    var argsResult = argsWithTypes
      .Where(x => x.left != null)
      .Select(x => DeserializeElement(x.right, x.left, path))
      .Collect();

    return argsResult.Map(args => constructor.Invoke(args.ToArray()))!;
  }

  /// <summary>
  /// Deserialize list
  /// </summary>
  /// <param name="typeHint"></param>
  /// <param name="list"></param>
  /// <param name="path"></param>
  /// <returns></returns>
  internal SerializationResult<object?> DeserializeList(Type? typeHint, RonList list, string path)
  {
    // Null checking
    typeHint = typeHint.NotNull(nameof(typeHint));

    // List element type.
    var listElementType = typeHint.GetListType().NotNull("typeHint.GetListType()")!;

    // Deserialize list elements.
    var elementsResult = list
      .Values.NotNull(nameof(list.Values))
      .WithIndex()
      .NotNull(nameof(list.Values))
      .Select((pair) => DeserializeElement(pair.value, listElementType, $"{path}[{pair.index}]"))
      .Collect();

    // Create the output array.
    var arrayResult = elementsResult.Map(elements =>
    {
      var array = Array.CreateInstance(listElementType, elements.Count());
      foreach (var (element, index) in elements.WithIndex())
      {
        array.SetValue(element, index);
      }
      return array;
    });

    // Attempt to initialize the type using an IEnumerable
    // This makes it possible to deserialize Lists and HashSets.
    return arrayResult.Map(
      (array) =>
      {
        if (
          typeHint.GetConstructor([typeof(IEnumerable<>).MakeGenericType(listElementType)])
          is ConstructorInfo cons
        )
        {
          return cons.Invoke([array])!;
        }

        // Return the raw array otherwise.
        return array;
      }
    )!;
  }

  /// <summary>
  /// Deserialize map
  /// </summary>
  /// <param name="map"></param>
  /// <param name="typeHint"></param>
  /// <param name="path"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  internal SerializationResult<object?> DeserializeMap(RonMap map, Type? typeHint, string path)
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

    List<Exception> errors = new List<Exception>();

    // Deserialize values.
    foreach (var element in map.Values.NotNull(nameof(map.Values)))
    {
      if (element is RonMapItem mapItem)
      {
        var key = mapItem.Key.AsNotNull<RonString>(nameof(mapItem.Key)).Evaluate();
        var value = DeserializeElement(mapItem.Value, valueType, $"{path}[\"{key}\"]");
        if (!value.isSuccess)
        {
          errors.Add(value.exception);
        }
        else
        {
          values.Add(key!, value.value);
        }
      }
    }

    if (errors.Count > 0)
    {
      return DeserializationException.FromOthers(errors).AsErr<object?>();
    }

    // Return the elements themselves if they are directly assignable to the output.
    if (values.GetType() == typeHint || typeHint == dictType)
    {
      return values.AsOk<object?>();
    }
    // Find a constructor for the dictionary type and invoke it.
    if (typeHint.GetConstructor([dictType]) is ConstructorInfo dictCons)
    {
      return dictCons.Invoke([values]).AsOk<object?>();
    }
    // Find a constructor that accepts a key value pair, and invoke it.
    if (
      typeHint.GetConstructor([
        typeof(IEnumerable<>).MakeGenericType([
          typeof(KeyValuePair<,>).MakeGenericType([typeof(string), valueType]),
        ]),
      ])
      is ConstructorInfo listCons
    )
    {
      return listCons.Invoke([values]).AsOk<object?>();
    }
    throw new NoDictionaryConstructorException(typeHint);
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
      return GetIntermediateType(result);
    }
    if (proxyTypes.TryFindByKey((x) => x.Name == name, out var proxyResult))
    {
      return proxyResult;
    }
    if (parentContext?.GetTypeFromName(name, backupType) is Type type)
    {
      return GetIntermediateType(type);
    }
    if (name != null && name.ToLower() != backupType?.Name.ToLower())
    {
      throw new InvalidOperationException();
    }
    return GetIntermediateType(backupType);
  }

  /// <summary>
  /// Get intermediate deserialization type, such as for a proxy.
  /// </summary>
  /// <param name="targetType"></param>
  /// <returns></returns>
  public Type? GetIntermediateType(Type? targetType)
  {
    if (targetType == null)
    {
      return null;
    }
    if (proxyTypes.TryGetValue(targetType, out var proxyType))
    {
      return proxyType;
    }
    if (targetType.GetCustomAttribute<RonProxyAttribute>() is RonProxyAttribute proxyAttr)
    {
      return proxyAttr.Proxy;
    }
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
      RonString => typeof(string),
      INumberValue numVal => numVal.CSType(),
      RonDocument doc => InferType(doc.Value.NotNull("doc.value")),
      RonIdentifier ident => GetTypeFromName(ident.Name, null),
      RonRawIdentifier ident => GetTypeFromName(ident.Name, null),
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
      _ => throw new NotImplementedForArgumentTypeException(nameof(InferType), element.GetType()),
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

    return source.RonConvert(targetType);
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

internal class SerializationResult<T>
{
  public bool isSuccess;
  public T? value;
  public Exception exception = null!;

  internal static SerializationResult<T> Ok(T? value)
  {
    return new SerializationResult<T> { isSuccess = true, value = value };
  }

  internal static SerializationResult<T> Err(Exception exception)
  {
    return new SerializationResult<T> { isSuccess = false, exception = exception };
  }
}

internal static class SerializationExtensions
{
  internal static SerializationResult<T> AsOk<T>(this T value) => SerializationResult<T>.Ok(value);

  internal static SerializationResult<T> AsErr<T>(this Exception exception) =>
    SerializationResult<T>.Err(exception);

  internal static SerializationResult<IEnumerable<T>> Collect<T>(
    this IEnumerable<SerializationResult<T>> source
  )
  {
    if (source.All(x => x.isSuccess))
    {
      return SerializationResult<IEnumerable<T>>.Ok(source.Select(x => x.value)!);
    }
    return source
      .Where(x => !x.isSuccess)
      .Select(x => x.exception)
      .Transform(DeserializationException.FromOthers)
      .AsErr<IEnumerable<T>>();
  }

  internal static SerializationResult<U> Map<T, U>(
    this SerializationResult<T> result,
    Func<T, U> func
  )
  {
    if (result.isSuccess)
    {
      return SerializationResult<U>.Ok(func(result.value!));
    }
    else
    {
      return SerializationResult<U>.Err(result.exception);
    }
  }
}
