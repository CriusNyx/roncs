using DeepEqual.Syntax;
using RonCS;

namespace RonTests;

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

    var actual = Ron.Deserialize(Source, typeof(GameConfig));
    actual.ShouldDeepEqual(expected);
  }
}
