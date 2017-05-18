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
            
            if(FileExists("Deploy.ps1"))
            {
                Information("Copying Deploy.ps1...");
                CopyFile("Deploy.ps1", webApp.CombineWithFilePath("Deploy.ps1"));
            }
            else
                Warning("Deploy.ps1 not found!");
            
            if(FileExists("PreDeploy.ps1"))
            {
                Information("Copying PreDeploy.ps1...");
                CopyFile("PreDeploy.ps1", webApp.CombineWithFilePath("PreDeploy.ps1"));
            }
            else
                Warning("PreDeploy.ps1 not found!");

            if(FileExists("PostDeploy.ps1"))
            {
                Information("Copying PostDeploy.ps1...");
                CopyFile("PostDeploy.ps1", webApp.CombineWithFilePath("PostDeploy.ps1"));
            }
            else
                Warning("PostDeploy.ps1 not found!");
            
            var jsBuildDirSource = parameters.Paths.Directories.Source.Combine(webApp.GetDirectoryName()).Combine("Scripts/build");
            var jsBuildDirTarget = webApp.Combine("Scripts/build");

            if(DirectoryExists(jsBuildDirSource))
                CopyDirectory(jsBuildDirSource, jsBuildDirTarget);

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

    var releaseNumber = parameters.Version.GitVersion.MajorMinorPatch;
    if(!String.IsNullOrEmpty(parameters.Version.GitVersion.PreReleaseLabel))
        releaseNumber += string.Format("{0}-{1}{2}", releaseNumber, parameters.Version.GitVersion.PreReleaseLabel, parameters.Version.GitVersion.CommitsSinceVersionSourcePadded);

    var settings = new CreateReleaseSettings {
            Server = EnvironmentVariable(octopusUrlVariable),
            ApiKey = EnvironmentVariable(octopusApiKeyVariable),
            EnableServiceMessages = true,
            ReleaseNumber = string.Format("{0}-{1}{2}", parameters.Version.GitVersion.MajorMinorPatch, parameters.Version.GitVersion.PreReleaseLabel, parameters.Version.GitVersion.CommitsSinceVersionSourcePadded),
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