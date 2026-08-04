using DeepEqual.Syntax;
using RonCS;

namespace RonTests;

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

public class GameConfigTests
{
  public static string Source =
    @"GameConfig( // optional struct name
    window_size: (800, 600),
    window_title: ""PAC-MAN"",
    fullscreen: true,

    mouse_sensitivity: 1.4,
    key_bindings: {
        ""up"": Up,
        ""down"": Down,
        ""left"": Left,
        ""right"": Right,

        // Uncomment to enable WASD controls
        /*
        ""W"": Up,
        ""S"": Down,
        ""A"": Left,
        ""D"": Right,
        */
    },

    difficulty_options: (
        start_difficulty: Easy,
        adaptive: false,
    ),
)";

  [Test]
  public void CanParseGameConfig()
  {
    var expected = new GameConfig
    {
      window_size = new(800, 600),
      window_title = "PAC-MAN",
      fullscreen = true,
      mouse_sensitivity = 1.4f,
      key_bindings = new Dictionary<string, InputAction>()
      {
        { "up", InputAction.Up },
        { "down", InputAction.Down },
        { "left", InputAction.Left },
        { "right", InputAction.Right },
      },
      difficulty_options = new DifficultyOptions
      {
        start_difficulty = Difficulty.Easy,
        adaptive = false,
      },
    };

    var actual = Ron.Deserialize(Ron.Parse(Source), typeof(GameConfig));
    actual.ShouldDeepEqual(expected);
  }
}
