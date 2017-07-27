#addin "Cake.Incubator"

///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////

#tool "nuget:?package=OctopusTools"

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////
Task("CreateOctoPackage")
    .Description("Creates a octopus package for published websites or the msbuild OutDir for non-web apps. Depends on tasks Build, Test and Transform.")
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
            
            if(config.OctopusReleaseOptions != null && config.OctopusReleaseOptions.AdditionalPackageFiles != null)
            {
                foreach(var apf in config.OctopusReleaseOptions.AdditionalPackageFiles)
                {
                    var source = parameters.Paths.Directories.Source.Combine(webApp.GetDirectoryName()).Combine(apf.Source);
                    var target = webApp.Combine(apf.Target);
                    if(DirectoryExists(source))
                    {
                        CopyDirectory(source, target);
                        Information("Finished copy of additional files: {0} -> {1}", source, target);
                    }
                    else
                        Warning("{0} not found!", source);
                        
                }    
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
    .Description("Creates an octopus release using the output from CreateOctoPackage. Depnds on task CreateOctoPackage.")
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

    var releaseNumber = BuildVersion.ReleaseNumber(parameters.Version.GitVersion);

    if(config.OctopusReleaseOptions != null && !String.IsNullOrEmpty(config.OctopusReleaseOptions.ReleaseNumberFormat))
    {
        releaseNumber = config.OctopusReleaseOptions.ReleaseNumberFormat
            .Replace("{MajorMinorPatch}", parameters.Version.GitVersion.MajorMinorPatch.ToString())
            .Replace("{Major}", parameters.Version.GitVersion.Major.ToString())
            .Replace("{Minor}", parameters.Version.GitVersion.Minor.ToString())
            .Replace("{Patch}", parameters.Version.GitVersion.Patch.ToString());
    }

    var settings = new CreateReleaseSettings {
            Server = EnvironmentVariable(octopusUrlVariable),
            ApiKey = EnvironmentVariable(octopusApiKeyVariable),
            EnableServiceMessages = true,
            ReleaseNumber = releaseNumber,
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