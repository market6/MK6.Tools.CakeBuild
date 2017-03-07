///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var parameters = BuildParameters.GetParameters(Context, BuildSystem);//, repositoryOwner, repositoryName);
var publishingError = false;

var config = Config.BuildJsonConfig("build.json");
string sourceDirectoryPath = config.SourceDirectoryPath;
string title = config.Title;
string solutionFilePath = config.SolutionFilePath;
string solutionDirectoryPath = config.SolutionDirectoryPath;
string octopusProjectName = config.OctopusProjectName;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    // if(parameters.IsMasterBranch && (context.Log.Verbosity != Verbosity.Diagnostic)) {
    //     Information("Increasing verbosity to diagnostic.");
    //     context.Log.Verbosity = Verbosity.Diagnostic;
    // }

    parameters.SetBuildPaths(
        BuildPaths.GetPaths(sourceDirectoryPath,
            context: Context
        )
    );

    parameters.SetBuildVersion(
        BuildVersion.CalculatingSemanticVersion(
            context: Context,
            parameters: parameters
        )
    );

    Information("Building version {0} of " + title + " ({1}, {2}) using version {3} of Cake.)",
        parameters.Version.SemVersion,
        parameters.Configuration,
        parameters.Target,
        parameters.Version.CakeVersion);
});

Teardown(context =>
{
    if(context.Successful)
    {
        // if(!parameters.IsLocalBuild && !parameters.IsPullRequest && parameters.IsMainRepository && parameters.IsMasterBranch && parameters.IsTagged)
        // {
        //     if(sendMessageToTwitter)
        //     {
        //         SendMessageToTwitter("Version " + parameters.Version.SemVersion + " of " + title + " Addin has just been released, https://www.nuget.org/packages/" + title + ".");
        //     }

        //     if(sendMessageToGitterRoom)
        //     {
        //         SendMessageToGitterRoom("@/all Version " + parameters.Version.SemVersion + " of the " + title + " Addin has just been released, https://www.nuget.org/packages/" + title + ".");
        //     }
        // }
    }
    else
    {
        // if(!parameters.IsLocalBuild && parameters.IsMainRepository)
        // {
        //     if(sendMessageToSlackChannel)
        //     {
        //         SendMessageToSlackChannel("Continuous Integration Build of " + title + " just failed :-(");
        //     }
        // }
    }

    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Show-Info")
    .Does(() =>
{
    Information("Target: {0}", parameters.Target);
    Information("Configuration: {0}", parameters.Configuration);

    Information("Solution FilePath: {0}", MakeAbsolute((FilePath)solutionFilePath));
    Information("Solution DirectoryPath: {0}", MakeAbsolute((DirectoryPath)solutionDirectoryPath));
    Information("Source DirectoryPath: {0}", MakeAbsolute(parameters.Paths.Directories.Source));
    Information("Build DirectoryPath: {0}", MakeAbsolute(parameters.Paths.Directories.Build));
});

Task("Clean")
    .Does(() =>
{
    Information("Cleaning...");

    CleanDirectories(parameters.Paths.Directories.ToClean);
});

Task("Restore")
    .Does(() =>
{
    Information("Restoring {0}...", solutionFilePath);
    IList<string> source;
    var sourceString = EnvironmentVariable(nuGetSourcesVariable);
    if(string.IsNullOrEmpty(sourceString))
        source = new List<string> { "https://www.nuget.org/api/v2", "http://packages.mk6.local/api/odata" };
    else
    {
        Verbose("Using nuget source(s) from env variable {0} for restore: {1}", nuGetSourcesVariable, sourceString);
        source = sourceString.Split(';').ToList();
    }
    NuGetRestore(solutionFilePath, new NuGetRestoreSettings { Source = source });
});

Task("Build")
    .IsDependentOn("Show-Info")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    Information("Building {0}", solutionFilePath);

    // TODO: Need to have an XBuild step here as well
    MSBuild(solutionFilePath, settings =>
        settings.SetPlatformTarget(PlatformTarget.MSIL)
            .WithProperty("TreatWarningsAsErrors","true")
            .WithProperty("OutDir", MakeAbsolute(parameters.Paths.Directories.TempBuild).FullPath)
            .WithTarget("Build")
            .SetConfiguration(parameters.Configuration));
});

Task("Package")
    .IsDependentOn("Create-NuGet-Packages")
    .IsDependentOn("Test");

Task("Default")
    .IsDependentOn("Package");

Task("ReleaseNotes");

Task("ClearCache");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(parameters.Target);