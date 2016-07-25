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

Task("UpdateAssemblyInfo")
    .IsDependentOn("CoreUpdateAssemblyInfo")
    .Does(() =>
{
      
});