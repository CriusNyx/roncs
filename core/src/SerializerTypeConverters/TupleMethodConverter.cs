using System.Reflection;
using RonCS.AST;

namespace RonCS;

/// <summary>
/// Type converter for a method that returns a tuple.
/// </summary>
/// <param name="name"></param>
/// <param name="method"></param>
public class TupleMethodConverter(string name, MethodInfo method) : TypeSerializerConverter
{
  public RonElement ToAST(SerializationContext context, object source)
  {
    var result = method
      .Invoke(source, [])
      .AsNotNull<object[]>($"{source.GetType()}.${method.Name}");
    return new RonTupleStruct(
      new RonIdentifier(name),
      new RonTuple(result.Select(x => context.ToAST(x)).ToArray())
    );
  }
}

/// <summary>
/// Type converter for a property that returns a tuple.
/// </summary>
/// <param name="name"></param>
/// <param name="property"></param>
public class TuplePropertyConverter(string name, PropertyInfo property) : TypeSerializerConverter
{
  public RonElement ToAST(SerializationContext context, object source)
  {
    var result = property
      .GetValue(source)
      .AsNotNull<object[]>($"{source.GetType()}.{property.Name}");
    return new RonTupleStruct(
      new RonIdentifier(name),
      new RonTuple(result.Select(x => context.ToAST(x)).ToArray())
    );
  }
}

/// <summary>
/// Type converter for a function that returns a tuple.
/// </summary>
/// <param name="name"></param>
/// <param name="converter"></param>
public class TupleFunctionConverter(string name, Func<object, object[]> converter)
  : TypeSerializerConverter
{
  public RonElement ToAST(SerializationContext context, object source)
  {
    var result = converter.Invoke(source).AsNotNull<object[]>($"{source.GetType()}.${name}");
    return new RonTupleStruct(
      new RonIdentifier(name),
      new RonTuple(result.Select(x => context.ToAST(x)).ToArray())
    );
  }
}
