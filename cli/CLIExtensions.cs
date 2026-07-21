using System.CommandLine;

public static class CLIExtensions
{
  public static T WithAction<T>(this T command, Action<ParseResult> action)
    where T : Command
  {
    command.SetAction(action);
    return command;
  }
}
