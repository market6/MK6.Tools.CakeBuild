#addin "Cake.FileHelpers"

const string artifactsPath = "./artifacts/"; //TODO: FIX ME: Move this to env variable and default to ./artifacts
var msBuildSettings = new MSBuildSettings()
                            .SetConfiguration(buildParams.Configuration)
                            .SetVerbosity(Verbosity.Minimal)
                            .UseToolVersion(MSBuildToolVersion.VS2015);

Task("CoreClean")
    .Does(() => 
    {
        DeleteFiles("./*.nupkg");
    });

Task("CoreRestoreNuGetPackages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var parsedSolution = ParseSolution(buildParams.SolutionPath);    
    foreach(var project in parsedSolution.Projects.Where(x => x.Type == "{8BB2217D-0F2D-49D1-97BC-3654ED321F3B}"))
    {
        if(HasEnvironmentVariable("NUGET_SOURCES")) 
        {
            var sources = new List<string>(EnvironmentVariable("NUGET_SOURCES").Split(';'));
            //NuGetRestore(buildParams.SolutionPath, new NuGetRestoreSettings { Source = sources });
            var settings = new DotNetCoreRestoreSettings
            {
                Sources = sources,
            };
                        
            DotNetCoreRestore(project.Path.GetDirectory().FullPath, settings);
        }
        else
            DotNetCoreRestore(project.Path.GetDirectory().FullPath);
    }
});

Task("SetTeamCityBuildNumber")
	.WithCriteria(TeamCity.IsRunningOnTeamCity)
	.Does(() =>
{
	TeamCity.SetBuildNumber(buildParams.GitVersion.FullSemVer);
});

Task("CoreBuild")
    .IsDependentOn("SetTeamCityBuildNumber")
    .IsDependentOn("RestoreNuGetPackages")
    .IsDependentOn("UpdateAssemblyInfo")
    .Does(() => DotNetCoreBuild(buildParams.SolutionPath, new DotNetCoreBuildSettings { Configuration = buildParams.Configuration }));

Task("CorePackage")
    .IsDependentOn("Build")
    .Does(() =>
{

    var parsedSolution = ParseSolution(buildParams.SolutionPath);
    foreach(var project in parsedSolution.Projects)
    {
        Verbose("XmlPeek " + project.Path);
        var outputType = XmlPeek(project.Path, "/Project/PropertyGroup/OutputType/text()");
        Verbose("outputType: " + outputType ?? "");
        if(!string.IsNullOrEmpty(outputType) && outputType.Equals("EXE", StringComparison.OrdinalIgnoreCase))
        {
            var settings = new DotNetCorePackSettings
            {
                Configuration = buildParams.Configuration,
                OutputDirectory = artifactsPath
            };
                        
            DotNetCorePack(project.Path.GetDirectory().FullPath, settings);    
        }
    }

    
});

Task("CorePublish")
    .IsDependentOn("Package")
    .Does(() =>
{
    var parsedSolution = ParseSolution(buildParams.SolutionPath);
    var filter = MakeAbsolute(Directory(artifactsPath).Path) + "/*.nupkg";
    var nupkgs = GetFiles(filter).Where(fi => parsedSolution.Projects.Any(p => fi.GetFilename().ToString().StartsWith(p.Name)));
    //Information(nupkgs.Count().ToString());
    NuGetPush(nupkgs, new NuGetPushSettings {
            ApiKey = EnvironmentVariable("NUGET_PUSH_API_KEY"),
            Source = EnvironmentVariable("NUGET_PUSH_SOURCE")
        });    
});

Task("CoreUpdateVersion")
    .Does(() =>
{
    var parsedSolution = ParseSolution(buildParams.SolutionPath);
    foreach(var project in parsedSolution.Projects)
    {
        Verbose("XmlPoke " + project.Path + " with version " + buildParams.PackageVersion);
        XmlPoke(project.Path, "/Project/PropertyGroup/VersionPrefix", buildParams.PackageVersion);
    }
    
});

