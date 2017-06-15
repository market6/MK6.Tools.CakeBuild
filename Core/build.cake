#tool "nuget:?package=GitVersion.CommandLine"

GitVersion version;
var target = Argument("target", "Default");

Setup(context =>
{
    version = context.GitVersion();
    context.Information("Version: {0}", version.FullSemVer);
    context.Information("Package Version: {0}", version.NuGetVersionV2);
});

Task("Package")
    .Does(() =>
{
    DeleteFiles("*.nupkg");
    NuGetPack("./src/MK6.Tools.CakeBuild.Core.nuspec", new NuGetPackSettings { Version = version.NuGetVersionV2, NoPackageAnalysis = true});
});

Task("Publish")
    .IsDependentOn("Package")
    .Does(() =>
{
    var pushSource = HasArgument("source") ? Argument<string>("source") : EnvironmentVariable("NUGET_DOT_ORG_URL");
    var apiKey =  HasArgument("apiKey") ? Argument<string>("apiKey") : EnvironmentVariable("NUGET_DOT_ORG_API_KEY");
    var nupkgPath = string.Format("./MK6.Tools.CakeBuild.Core.{0}.nupkg", version.NuGetVersionV2);
    
    Information("pushSource: {0}", pushSource);
    Information("apiKey: {0}", apiKey);

    NuGetPush(nupkgPath, new NuGetPushSettings {
        ApiKey = apiKey,
        Source = pushSource
    });
});

Task("Default")
  .IsDependentOn("Package")
  .Does(() =>
{

});

RunTarget(target);