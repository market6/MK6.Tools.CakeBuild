
BuildParams buildParams = BuildParams.GetParams(Context);
// Install addins.

// Include Cake files
#load "build/core.dotnet.cake" 
#load "build/params.cake"

Task("Clean")
    .IsDependentOn("CoreClean")
    .Does(() => {

    });

Task("RestoreNuGetPackages")
    .IsDependentOn("CoreRestoreNuGetPackages")
    .Does(() =>
{
   
});

Task("Build")
    .IsDependentOn("CoreBuild")
    .Does(() =>
{
    
});

Task("Package")
    .IsDependentOn("CorePackage")
    .Does(() =>
{

});

Task("Publish")
    .IsDependentOn("CorePublish")
    .Does(() =>
{
      
});

Task("Default")
  .IsDependentOn("Build")
  .Does(() =>
{

});

RunTarget(buildParams.Target);