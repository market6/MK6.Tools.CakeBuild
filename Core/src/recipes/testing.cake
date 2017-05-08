///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////



///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Test-VSTest")
    .Does(() =>
{
    var testsPattern = parameters.Paths.Directories.TempBuild + "/" + testAssemblySearchPattern;
    if(GetFiles(testsPattern).Any())
    {
        Verbose("testsPattern: {0}", testsPattern);
        VSTest(testsPattern, new VSTestSettings().WithVisualStudioLogger());
        if(TeamCity.IsRunningOnTeamCity)
        {
            var trxPath = GetFiles(parameters.Paths.Directories.TestResults.FullPath + "/*.trx");
            TeamCity.ImportData("vstest", string.Format("./TestResults/{0}", trxPath.First().GetFilename()));
        }
    }
    {
        Warning("No tests found to run.");
    }
});

Task("Test")
    .IsDependentOn("Build")
    .IsDependentOn("Test-VSTest")
    .Does(() =>
{
});