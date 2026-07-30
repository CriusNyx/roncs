using System.Reflection;
using CriusNyx.Util;
using RonCS;

namespace RonTests;

public class TestFileTests
{
  [DatapointSource]
  public IEnumerable<string> filePaths
  {
    get
    {
      var assemblyLoc = Assembly.GetCallingAssembly().Location;
      var testFileDir = Path.Join(assemblyLoc, "../testFiles");
      return Directory.GetFiles(testFileDir, "*.ron");
    }
  }

  [Theory]
  public void TestFile_DebugASTs_Match(string path)
  {
    var ronSource = File.ReadAllText(path);
    var parsed = Ron.Parse(ronSource);
    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
    var debugFileName = $"{fileNameWithoutExtension}.ast.debug";
    var debugFilePath = Path.Join(path, "..", debugFileName);
    var debugFileText = File.ReadAllText(debugFilePath);
    Assert.That(parsed.Debug(), Is.EqualTo(debugFileText));
  }
}
