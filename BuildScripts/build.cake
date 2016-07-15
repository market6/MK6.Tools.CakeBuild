var version = Argument("appVer", "1.0.0");
var target = Argument("target", "Default");

Task("Package")
    .Does(() =>
{
    NuGetPack("./src/Tools.CakeBuild.nuspec", new NuGetPackSettings { Version = version, NoPackageAnalysis = true});
});

Task("Publish")
    .IsDependentOn("Package")
    .Does(() =>
{
    var pushSource = Argument<string>("source");
    var apiKey = Argument<string>("apiKey");

    var nupkgPath = string.Format("./Tools.CakeBuild.{0}.nupkg", version);
    //Information("pushing package " + nupkgPath);
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