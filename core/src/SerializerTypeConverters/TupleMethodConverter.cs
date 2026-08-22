using System.Reflection;
using CriusNyx.Util;

namespace RonCS;

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
