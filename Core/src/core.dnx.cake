#addin "Cake.FileHelpers"
#addin "Cake.Json"
using Newtonsoft.Json;

const string artifactsPath = "./artifacts/"; //TODO: FIX ME: Move this to env variable and default to ./artifacts
var msBuildSettings = new MSBuildSettings()
                            .SetConfiguration(buildParams.Configuration)
                            .SetVerbosity(Verbosity.Minimal)
                            .UseToolVersion(MSBuildToolVersion.VS2015);

Task("CoreClean")
    .Does(() => 
    {
        MSBuild(buildParams.SolutionPath, msBuildSettings.WithTarget("Clean"));
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
	//TeamCity.SetBuildNumber(buildParams.GitVersion.FullSemVer);
});

Task("CoreBuild")
    .IsDependentOn("SetTeamCityBuildNumber")
    .IsDependentOn("RestoreNuGetPackages")
    .IsDependentOn("UpdateAssemblyInfo")
    .Does(() => MSBuild(buildParams.SolutionPath, msBuildSettings.WithTarget("Build")));

Task("CorePackage")
    .IsDependentOn("Build")
    .Does(() =>
{
    foreach(var nuspec in buildParams.NuspecPaths)
    {
        NuGetPack(nuspec.FullPath, new NuGetPackSettings {
                                    Version = buildParams.Version,
                                    Properties = new Dictionary<string, string>{ {"configuration", buildParams.Configuration} },
                                    NoPackageAnalysis = true
        });
    }
});

Task("CoreDNXPackage")
    .IsDependentOn("Build")
    .Does(() =>
{

    var parsedSolution = ParseSolution(buildParams.SolutionPath);
    foreach(var project in parsedSolution.Projects.Where(x => x.Type == "{8BB2217D-0F2D-49D1-97BC-3654ED321F3B}"))
    {
        var settings = new DotNetCorePackSettings
        {
            Configuration = buildParams.Configuration,
            OutputDirectory = artifactsPath
        };
                    
        DotNetCorePack(project.Path.GetDirectory().FullPath, settings);    
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

Task("CoreUpdateAssemblyInfo")
    .Does(() =>
{
	Newtonsoft.Json.JsonConvert.DefaultSettings = () => new JsonSerializerSettings
	{
		Formatting = Newtonsoft.Json.Formatting.Indented
	};

    var parsedSolution = ParseSolution(buildParams.SolutionPath);
    foreach(var project in parsedSolution.Projects.Where(x => x.Type == "{8BB2217D-0F2D-49D1-97BC-3654ED321F3B}"))
    {
        var projectJson = project.Path.GetDirectory().CombineWithFilePath("project.json");
        Information("Looking for project.json: {0}", projectJson);
        if(FileExists(projectJson))
        {
            Information("Found project.json: {0}", projectJson);
            Information("Parsing project.json...");
            var jo = ParseJsonFromFile(projectJson);
            Information("Setting version to: {0}...", buildParams.PackageVersion);
            jo["version"] = buildParams.PackageVersion;
            Information("Updating project.json...");
            SerializeJsonToFile(projectJson, jo);
        }

    }
});

