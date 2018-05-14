//#tool nuget:?package=Codecov
//#addin nuget:?package=Cake.Codecov

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var isLocalBuild        = !AppVeyor.IsRunningOnAppVeyor;
var packPath            = Directory("./src/AB.Extensions");
var sourcePath          = Directory("./src/AB.Extensions");
var testsPath           = Directory("./test/AB.Extensions.Tests");
var buildArtifacts      = Directory("./artifacts/packages");
var configuration   = Argument<string>("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Build")
    .IsDependentOn("Clean")
    //.IsDependentOn("Version")
    .IsDependentOn("Restore")
    .Does(() => {
	var settings = new DotNetCoreBuildSettings 
        {
            Configuration = configuration
            // Runtime = IsRunningOnWindows() ? null : "unix-x64"
        };
        DotNetCoreBuild("./", settings);
    });

Task("Clean")
    .Does(() => 
	{
        CleanDirectories(new DirectoryPath[] { buildArtifacts });
    });

//Task("Restore")
//    .Does(() => 
//	{
//        DotNetCoreRestore("src");
//    });

Task("Restore")
    .Does(() =>
	{
	    var settings = new DotNetCoreRestoreSettings
	    {
	        Sources = new [] { "https://api.nuget.org/v3/index.json" }
	    };
	
	    DotNetCoreRestore("./", settings);
	    DotNetCoreRestore(testsPath, settings);
	});

Task("RunTests")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does( () =>
	{   
		var settings = new DotNetCoreTestSettings { Configuration = configuration };
	    DotNetCoreTest("./test/AB.Extensions.Tests/AB.Extensions.Tests.csproj", settings);
	});

Task("Package")
    .IsDependentOn("Build")
    .Does(() => {
        var packSettings = new DotNetCorePackSettings
        {
            OutputDirectory = buildArtifacts,
            NoBuild = true
        };
 
         DotNetCorePack(buildArtifacts);
 
    });
//Task("UploadCodeCoverage")
    //.IsDependentOn("RunTests")    
    //.Does( () =>
	//{   
	//	var settings = new CodecovSettings();
	//	settings.Dump = true; // more @ http://cakebuild.net/api/Cake.Codecov/CodecovSettings/
	//	settings.Verbose = true;
	//	settings.Files = new[] { "coverage.xml" };
	 //   Codecov(settings);
	//});
	//{
    // Upload a coverage report.
    //Codecov("coverage.xml");
//});

Task("Default")
  .IsDependentOn("Build")
  .IsDependentOn("RunTests")
  //.IsDependentOn("UploadCodeCoverage");
  .IsDependentOn("Package");

RunTarget(target);