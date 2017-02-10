#load "tools/MK6.Tools.CakeBuild.Core/core.params.cake"

BuildParams buildParams = BuildParams.GetParams(Context);

// Install addins.

// Include Additional Cake files
#load "tools/MK6.Tools.CakeBuild.Core/core.netcore.cake" 

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

Task("LibPackage")
    .IsDependentOn("CoreLibPackage")
    .Does(() =>
{

});


Task("AppPublish")
    .IsDependentOn("CoreAppPublish")
    .Does(() =>
{
      
});

Task("LibPublish")
    .IsDependentOn("CoreAppPublish")
    .Does(() =>
{
      
});

Task("UpdateAssemblyInfo")
    .IsDependentOn("CoreUpdateVersion")
    .Does(() =>
{
      
});

Task("Default")
  .IsDependentOn("Build")
  .Does(() =>
{

});

RunTarget(buildParams.Target);