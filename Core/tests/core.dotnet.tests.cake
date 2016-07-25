#load "../src/core.params.cake"

BuildParams buildParams = BuildParams.GetParams(Context);

// Install addins.

// Include Additional Cake files
#load "../src/core.dotnet.cake" 
#load "utils/core.tasks.cake"

Task("Test-CoreUpdateAssemblyInfo")
    .Does(() => {
      buildParams.Version = "9.9.9";
      Environment.SetEnvironmentVariable("ASSEMBLY_INFO_COMPANY", "TestCo, Inc.");
      RunTarget("CoreUpdateAssemblyInfo");
});

Task("Default")
  .IsDependentOn("Test-CoreUpdateAssemblyInfo")
  .Does(() => {});

RunTarget("Default");