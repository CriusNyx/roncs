import * as Path from "jsr:@std/path";
import * as FS from "jsr:@std/fs";

interface RunOpts {
  cwd?: string;
}

async function run(command: string, args: string[], opts?: RunOpts) {
  const commandObj = new Deno.Command(command, {
    args,
    cwd: opts?.cwd,
    stdin: "inherit",
    stdout: "inherit",
    stderr: "inherit",
  });
  return await commandObj.output();
}

async function ensureDir(dirName: string) {
  FS.ensureDir(dirName);
  return dirName;
}

async function main() {
  const scriptPath = import.meta.dirname;
  if (!scriptPath) {
    throw "Should be impossible.";
  }

  // File paths
  const docsFileName = "RonCS.xml";
  const searchCacheName = "RonCS.searchCache.json";
  const routeCacheTargetName = "RonCS.routeCache.json";
  const generatedCodePath = await ensureDir(
    Path.join(scriptPath, "../intro/generated"),
  );
  const cliPath = Path.join(scriptPath, "../cli");

  const ronCSTargetPath = Path.join(generatedCodePath, docsFileName);
  const searchCacheTargetPath = Path.join(generatedCodePath, searchCacheName);
  const routeCacheTargetPath = Path.join(
    generatedCodePath,
    routeCacheTargetName,
  );

  // Run C# documentation generator.
  const generateXMLDocsResult = await run(
    "dotnet",
    ["run", "GenerateXMLDocs", ronCSTargetPath],

    { cwd: cliPath },
  );

  if (generateXMLDocsResult.code) {
    throw `Build failed with exit code ${generateXMLDocsResult.code}`;
  } else {
    console.log("Wrote RonCS.xml");
  }

  const searchCacheResult = await run(
    "dotnet",
    ["run", "GenerateSearchCache", searchCacheTargetPath],

    { cwd: cliPath },
  );

  if (searchCacheResult.code) {
    throw `Build failed with exit code ${searchCacheTargetPath}`;
  } else {
    console.log("Wrote RonCS.searchCache.json");
  }

  const routeCacheResult = await run(
    "dotnet",
    ["run", "GenerateRouteCache", routeCacheTargetPath],
    { cwd: cliPath },
  );

  if (routeCacheResult.code) {
    throw `Build failed with exit code ${routeCacheResult.code}`;
  } else {
    console.log("Wrote RonCS.routeCache.json");
  }
}

await main();
