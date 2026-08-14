using System.Collections;
using System.Text.RegularExpressions;
using CriusNyx.Util.Reflection;

namespace RonTests;

public class EmptyClass { }

public class SimpleClass
{
  public string foo = null!;
  public int bar;
}

public class NestedClass
{
  public SimpleClass simpleClass = null!;
}

public class ParentClass { }

public class ChildClassA : ParentClass
{
  public string foo = null!;
}

public class ChildClassB : ParentClass
{
  public string bar = null!;
}

public class Vector3()
{
  public float x;
  public float y;
  public float z;

  public Vector3(float x, float y, float z)
    : this()
  {
    this.x = x;
    this.y = y;
    this.z = z;
  }

  public static Vector3 Parse(string source)
  {
    var regex = @"\(\s*([^,]*)\s*,\s*([^,]*)\s*,\s*([^,]*)\s*\)";
    var groups = Regex.Match(source, regex).Groups;

    var x = float.Parse(groups[1].Value);
    var y = float.Parse(groups[2].Value);
    var z = float.Parse(groups[3].Value);
    return new Vector3(x, y, z);
  }

  public override string ToString()
  {
    return $"({x}, {y}, {z})";
  }
}

public class VectorList
{
  public IEnumerable<Vector3> values = null!;
}

public class WithDict
{
  public Dictionary<string, Vector3> values = null!;
}

public class CreateWithDict(IDictionary<string, Vector3> values = null!)
{
  public IDictionary<string, Vector3> values = values;
}

public class Vector2Int()
{
  public int x;
  public int y;

  public Vector2Int(int x, int y)
    : this()
  {
    this.x = x;
    this.y = y;
  }
}

public enum InputAction
{
  Up,
  Down,
  Left,
  Right,
}

public enum Difficulty
{
  Easy,
}

public class DifficultyOptions
{
  public Difficulty start_difficulty;
  public bool adaptive;
}

public class GameConfig
{
  public Vector2Int window_size = null!;
  public string window_title = null!;
  public bool fullscreen;
  public float mouse_sensitivity;
  public Dictionary<string, InputAction> key_bindings = null!;
  public DifficultyOptions difficulty_options = null!;
}

[RonProxy(typeof(ProxyType))]
public class TypeWithProxy
{
  public Vector3? vector;
}

public class ProxyType
{
  public string? vectorValue;

  [RonFrom]
  public static ProxyType From(TypeWithProxy source)
  {
    return new ProxyType { vectorValue = source.vector?.ToString() };
  }

  [RonInto]
  public static TypeWithProxy IntoSource(ProxyType self)
  {
    return new TypeWithProxy { vector = Vector3.Parse(self.vectorValue!) };
  }
}

internal class StringListEnumerator(StringList? cursor) : IEnumerator<string>
{
  StringList? head = cursor;
  StringList? cursor = cursor;
  public string Current { get; private set; } = null!;

  object IEnumerator.Current => Current;

  public void Dispose() { }

  public bool MoveNext()
  {
    if (cursor == null)
    {
      return false;
    }
    Current = cursor.value;
    cursor = cursor.next;
    return true;
  }

  public void Reset()
  {
    cursor = head;
    Current = null!;
  }
}

[RonList]
public class StringList : IEnumerable<string>
{
  public string value;
  public StringList? next;

  public StringList(string value, StringList? next = null)
  {
    this.value = value;
    this.next = next;
  }

  public IEnumerator<string> GetEnumerator()
  {
    return new StringListEnumerator(this);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return new StringListEnumerator(this);
  }

  [RonFrom]
  public static StringList? Create(IEnumerable<string> values)
  {
    return values
      .Reverse()
      .Aggregate(null as StringList, (prev, curr) => new StringList(curr, prev));
  }
}
