using System.CommandLine;
using CriusNyx.Util;

void GenerateRegressionTests(ParseResult parsed)
{
  var cwd = Directory.GetCurrentDirectory();
  if (!Directory.GetFiles(cwd).Any(file => Path.GetFileName(file) == "cli.csproj"))
  {
    throw new InvalidOperationException("This must be called from the the location of cil.csproj");
  }

  var testFiles = Directory.GetFiles(Path.Join(cwd, "../tests/testFiles"), "*.ron");

  foreach (var testFile in testFiles)
  {
    var text = File.ReadAllText(testFile).Trim();
    var ronDoc = Ron.Parse(text);
    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(testFile);
    var newFileName = $"{fileNameWithoutExtension}.ast.debug";
    var newFilePath = Path.Join(testFile, $"../{newFileName}");
    File.WriteAllText(newFilePath, ronDoc.Debug());
  }
}

new RootCommand("RonCS CLI")
{
  Subcommands =
  {
    new Command("GenerateRegressionTests", "Generate Regression Tests").WithAction(
      GenerateRegressionTests
    ),
  },
}
  .WithAction((parsed) => { })
  .Parse(args)
  .Invoke();
