using System.Reflection;
using CriusNyx.Util;
using RonCS;
using RonCS.AST;

namespace RonTests;

public class TestFileTests
{
  [OneTimeSetUp]
  public static void Setup()
  {
    DebugPrint.RegisterCustomType<RonDocument>(x => ["Value".With(x.Value)!]);

    DebugPrint.RegisterCustomType<RonUnitStruct>(x => []);
    DebugPrint.RegisterCustomType<RonTupleStruct>(x => ["Body".With(x.Body)!]);
    DebugPrint.RegisterCustomType<RonMapStruct>(x => ["MapBody".With(x.MapBody)!]);
    DebugPrint.RegisterCustomType<RonNamedValueStruct>(x => ["Body".With(x.Body)!]);

    DebugPrint.RegisterCustomType<RonTuple>(x => ["Values".With(x.Values)!]);
    DebugPrint.RegisterCustomType<RonMap>(x => ["Values".With(x.Values)!]);

    DebugPrint.RegisterCustomType<RonIdentifier>(x => ["Name".With(x.Name)!]);
    DebugPrint.RegisterCustomType<RonMapItem>(x => ["Key".With(x.Key)!, "Value".With(x.Value)!]);
    DebugPrint.RegisterCustomType<RonNamedValue>(x =>
      ["name".With(x.name)!, "value".With(x.value)!]
    );

    DebugPrint.RegisterCustomType<RonBool>(x => ["Value".With(x.Value)]);
  }

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
