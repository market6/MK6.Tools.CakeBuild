///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////

#tool "nuget:?package=OctopusTools"

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////
Task("CreateOctoPackage")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Transform")
    .Does(() =>
{
    var basePath = MakeAbsolute(parameters.Paths.Directories.TempBuild);
    var artifactsPath = MakeAbsolute(parameters.Paths.Directories.Build);
    Verbose("basePath: " + basePath.FullPath);	
    Verbose("octopusProjectName: " + octopusProjectName);	

    OctoPack(octopusProjectName, new OctopusPackSettings 
    {
        BasePath = basePath,
        Format = OctopusPackFormat.NuPkg,
        Version = parameters.Version.SemVersion,
        OutFolder = artifactsPath,
        Overwrite = true
    });
});

Task("CreateOctoRelease")
    .IsDependentOn("CreateOctoPackage")
    .Does(() =>
{
  var latestReleaseNotes = "";
	var releaseNotesPath = string.Format("{0}/ReleaseNotes.md", parameters.Paths.Directories.Source);
	if(FileExists(releaseNotesPath))
	{
        var releaseNote = ParseReleaseNotes(releaseNotesPath);
        if(parameters.Version.GitVersion.MajorMinorPatch == releaseNote.Version.ToString())
        {
          latestReleaseNotes = releaseNote.RawVersionLine + "\n";
          foreach(var note in releaseNote.Notes)
          {
              latestReleaseNotes += string.Format("* {0}\n", note);
          }
        }
	}

    OctoCreateRelease(octopusProjectName, new CreateReleaseSettings {
        Server = EnvironmentVariable(octopusUrlVariable),
        ApiKey = EnvironmentVariable(octopusApiKeyVariable),
        EnableServiceMessages = true,
		    ReleaseNumber = parameters.Version.SemVersion, //string.Format("{0}.i", parameters.Version.MajorMinorPatch),
        DefaultPackageVersion = parameters.Version.SemVersion,
		    ReleaseNotes = latestReleaseNotes
    });
});


///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////
