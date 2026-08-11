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
