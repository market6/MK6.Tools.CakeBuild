#addin "Cake.Incubator"

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
    EnsureDirectoryExists(parameters.Paths.Directories.NuGetPackages);
    var artifactsPath = MakeAbsolute(parameters.Paths.Directories.NuGetPackages);

    if(DirectoryExists(parameters.Paths.Directories.PublishedWebsites))
    {
        foreach(var webApp in GetDirectories(parameters.Paths.Directories.PublishedWebsites.FullPath + "/*"))
        {
            if(DirectoryExists(parameters.Paths.Directories.ConfigsDirectory))
                CopyDirectory(parameters.Paths.Directories.ConfigsDirectory, webApp.Combine(parameters.Paths.Directories.ConfigsDirectory.GetDirectoryName()));
            if(DirectoryExists(parameters.Paths.Directories.NativeLibsDirectory))
            {
                CopyDirectory(parameters.Paths.Directories.NativeLibsDirectory, webApp.Combine("bin").Combine(parameters.Paths.Directories.NativeLibsDirectory.GetDirectoryName()));
                DeleteDirectory(webApp.Combine(parameters.Paths.Directories.NativeLibsDirectory.GetDirectoryName()), true);
            }
            
            DeleteFiles(webApp.FullPath + "/Web.*.config");
            DeleteFiles(webApp.FullPath + "/Web.config");
            OctoPack(webApp.GetDirectoryName(), new OctopusPackSettings 
            {
                BasePath = MakeAbsolute(webApp),
                Format = OctopusPackFormat.NuPkg,
                Version = parameters.Version.SemVersion,
                OutFolder = artifactsPath,
                Overwrite = true
            });
        }
    }
    else
    { 
        OctoPack(octopusProjectName, new OctopusPackSettings 
        {
            BasePath = MakeAbsolute(parameters.Paths.Directories.TempBuild),
            Format = OctopusPackFormat.NuPkg,
            Version = parameters.Version.SemVersion,
            OutFolder = artifactsPath,
            Overwrite = true
        });
    }
    
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

    var releaseNumberFormat = String.IsNullOrEmpty(octopusReleaseNumberFormat) ?  "{NuGetVersionV2}" : octopusReleaseNumberFormat;

    Information("Building release number using format: {0}", releaseNumberFormat);

    var releaseNumber = BuildReleaseNumber(parameters.Version.GitVersion, releaseNumberFormat);
    
    Information("Using release number: {0}", releaseNumber);
    

    var settings = new CreateReleaseSettings {
            Server = EnvironmentVariable(octopusUrlVariable),
            ApiKey = EnvironmentVariable(octopusApiKeyVariable),
            EnableServiceMessages = true,
            ReleaseNumber = releaseNumber, //string.Format("{0}.i-{1}{2}", parameters.Version.GitVersion.MajorMinorPatch, parameters.Version.GitVersion.PreReleaseLabel, parameters.Version.GitVersion.CommitsSinceVersionSourcePadded),
            DefaultPackageVersion = parameters.Version.SemVersion,
            ReleaseNotes = latestReleaseNotes
        };

    if(parameters.IsLocalBuild)
    {
        Information(settings.Dump<CreateReleaseSettings>());
    }
    else
    {
        OctoCreateRelease(octopusProjectName, settings);
    }
});


///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////
using System.Reflection;

static string BuildReleaseNumber(GitVersion version, string releaseNumberFormat)
{
    //var releaseNumber = string.Empty;
    var properties = version.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => p.Name);

    foreach (var p in properties)
    {
        var token = "{" + p.Name + "}";
        releaseNumberFormat = releaseNumberFormat.Replace(token, p.GetValue(version).ToString());
    }

    return releaseNumberFormat;

}