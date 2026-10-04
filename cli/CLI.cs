using System.CommandLine;
using CriusNyx.Util;
using RonCS;

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

var outFileArgument = new Argument<string>("outFile");

new RootCommand("RonCS CLI")
{
  Subcommands =
  {
    new Command("GenerateRegressionTests", "Generate Regression Tests").WithAction(
      GenerateRegressionTests
    ),
    new Command("GenerateXMLDocs", "Generate Xml Docs")
      .WithArgument(outFileArgument)
      .WithAction(
        (parsedArgs) =>
          XMLDocGenerator.GenerateXMLDocsFile(
            parsedArgs.GetValue(outFileArgument).NotNull("outFile")
          )
      ),
    new Command("GenerateSearchCache", "Generate Search Cache")
      .WithArgument(outFileArgument)
      .WithAction(
        (parsedArgs) =>
          SearchCacheGenerator.GenerateSearchCacheFile(
            parsedArgs.GetValue(outFileArgument).NotNull("outFile")
          )
      ),
    new Command("GenerateRouteCache", "Generate Route Cache")
      .WithArgument(outFileArgument)
      .WithAction(
        (parsedArgs) =>
          RouteCacheGenerator.GenerateRouteCacheFile(
            parsedArgs.GetValue(outFileArgument).NotNull("outFile")
          )
      ),
  },
}
  .WithAction((parsed) => { })
  .Parse(args)
  .Invoke();

class Disposable(Action dispose) : IDisposable
{
  public void Dispose()
  {
    dispose();
  }
}
