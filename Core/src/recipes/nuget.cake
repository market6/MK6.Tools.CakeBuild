Task("Create-NuGet-Packages")
    .IsDependentOn("Build")
    .WithCriteria(() => DirectoryExists(parameters.Paths.Directories.NugetNuspecDirectory))
    .Does(() =>
{
    var nuspecFiles = GetFiles(parameters.Paths.Directories.NugetNuspecDirectory + "/**/*.nuspec");

    EnsureDirectoryExists(parameters.Paths.Directories.NuGetPackages);

    var enableSymbols = true;
    if(HasEnvironmentVariable(nugetEnableSymbols))
    {
        if(!bool.TryParse(EnvironmentVariable(nugetEnableSymbols), out enableSymbols))
            enableSymbols = true;
    }

    if(enableSymbols)
    {
        var srcDir = parameters.Paths.Directories.TempBuild.Combine(Directory("src"));
        EnsureDirectoryExists(srcDir);
        CopyFiles(parameters.Paths.Directories.Source.FullPath + "/**/*.cs", srcDir, true);
    }

    foreach(var nuspecFile in nuspecFiles)
    {
        // TODO: Addin the release notes
        // ReleaseNotes = parameters.ReleaseNotes.Notes.ToArray(),

        // Create packages.
        NuGetPack(nuspecFile, new NuGetPackSettings {
            BasePath = parameters.Paths.Directories.TempBuild,
            Version = parameters.Version.SemVersion,
            OutputDirectory = parameters.Paths.Directories.NuGetPackages,
            Symbols = enableSymbols,
            NoPackageAnalysis = true
        });
    }
});


Task("Publish-Nuget-Packages")
    .IsDependentOn("Package")
    //.WithCriteria(() => !parameters.IsLocalBuild)
    .WithCriteria(() => DirectoryExists(parameters.Paths.Directories.NuGetPackages))
    .Does(() =>
{
    if(string.IsNullOrEmpty(parameters.NuGet.ApiKey)) {
        throw new InvalidOperationException("Could not resolve NuGet API key.");
    }

    if(string.IsNullOrEmpty(parameters.NuGet.SourceUrl)) {
        throw new InvalidOperationException("Could not resolve NuGet API url.");
    }

    var nupkgFiles = GetFiles(parameters.Paths.Directories.NuGetPackages + "/**/*.nupkg");

    foreach(var nupkgFile in nupkgFiles)
    {
        Information("Using source {0}", parameters.NuGet.SourceUrl);
        // Push the package.
        NuGetPush(nupkgFile, new NuGetPushSettings {
            Source = parameters.NuGet.SourceUrl,
            ApiKey = parameters.NuGet.ApiKey
        });
    }
})
.OnError(exception =>
{
    Information("Publish-Nuget-Packages Task failed, but continuing with next Task...");
    Error(exception.ToString());
    publishingError = true;
});