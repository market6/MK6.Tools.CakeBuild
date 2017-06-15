public class BuildPaths
{
    public BuildFiles Files { get; private set; }
    public BuildDirectories Directories { get; private set; }

    public static BuildPaths GetPaths(DirectoryPath sourceDirectoryPath,
        ICakeContext context
        )
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        // Directories
        var buildDirectoryPath             = "./BuildArtifacts";
        var tempBuildDirectoryPath         = buildDirectoryPath + "/temp";
        var publishedWebsitesDirectory     = tempBuildDirectoryPath + "/_PublishedWebsites";
        var publishedApplicationsDirectory = tempBuildDirectoryPath + "/_PublishedApplications";
        var publishedLibrariesDirectory    = tempBuildDirectoryPath;//tempBuildDirectoryPath + "/_PublishedLibraries";
        var configsDirectory               = buildDirectoryPath + "/_configs";
        var nativeLibsDirectory            = tempBuildDirectoryPath + "/nativelibs";
        var nugetNuspecDirectory           = sourceDirectoryPath.FullPath + "/nuspec/nuget";
        var chocolateyNuspecDirectory      = "./nuspec/chocolatey";

        var testResultsDirectory        = "./TestResults";
        var inspectCodeResultsDirectory = testResultsDirectory + "/InspectCode";
        var dupFinderResultsDirectory   = testResultsDirectory + "/DupFinder";

        var testCoverageDirectory = buildDirectoryPath + "/TestCoverage";

        var nuGetPackagesOutputDirectory = buildDirectoryPath + "/Packages/NuGet";
        var chocolateyPackagesOutputDirectory = buildDirectoryPath + "/Packages/Chocolatey";

        // Files
        var testCoverageOutputFilePath = ((DirectoryPath)testCoverageDirectory).CombineWithFilePath("OpenCover.xml");
        var solutionInfoFilePath = ((DirectoryPath)sourceDirectoryPath).CombineWithFilePath("SolutionInfo.cs");

        var repoFilesPaths = new FilePath[] {
            "LICENSE",
            "README.md"
        };

        var buildDirectories = new BuildDirectories(
            sourceDirectoryPath,
            buildDirectoryPath,
            tempBuildDirectoryPath,
            publishedWebsitesDirectory,
            publishedApplicationsDirectory,
            publishedLibrariesDirectory,
            nugetNuspecDirectory,
            chocolateyNuspecDirectory,
            testResultsDirectory,
            nuGetPackagesOutputDirectory,
            chocolateyPackagesOutputDirectory,
            configsDirectory,
            nativeLibsDirectory
            );

        var buildFiles = new BuildFiles(
            context,
            repoFilesPaths,
            testCoverageOutputFilePath,
            solutionInfoFilePath
            );

        return new BuildPaths
        {
            Files = buildFiles,
            Directories = buildDirectories
        };
    }
}

public class BuildFiles
{
    public ICollection<FilePath> RepoFilesPaths { get; private set; }

    public FilePath TestCoverageOutputFilePath { get; private set; }

    public FilePath SolutionInfoFilePath { get; private set; }

    public BuildFiles(
        ICakeContext context,
        FilePath[] repoFilesPaths,
        FilePath testCoverageOutputFilePath,
        FilePath solutionInfoFilePath
        )
    {
        RepoFilesPaths = Filter(context, repoFilesPaths);
        TestCoverageOutputFilePath = testCoverageOutputFilePath;
        SolutionInfoFilePath = solutionInfoFilePath;
    }

    private static FilePath[] Filter(ICakeContext context, FilePath[] files)
    {
        // Not a perfect solution, but we need to filter PDB files
        // when building on an OS that's not Windows (since they don't exist there).

        if(!context.IsRunningOnWindows())
        {
            return files.Where(f => !f.FullPath.EndsWith("pdb")).ToArray();
        }

        return files;
    }
}

public class BuildDirectories
{
    public DirectoryPath Source { get; private set; }
    public DirectoryPath Build { get; private set; }
    public DirectoryPath TempBuild { get; private set; }
    public DirectoryPath PublishedWebsites { get; private set; }
    public DirectoryPath PublishedApplications { get; private set; }
    public DirectoryPath PublishedLibraries { get; private set; }
    public DirectoryPath NugetNuspecDirectory { get; private set; }
    public DirectoryPath ChocolateyNuspecDirectory { get; private set; }
    public DirectoryPath TestResults { get; private set; }
    public DirectoryPath TestCoverage { get; private set; }
    public DirectoryPath NuGetPackages { get; private set; }
    public DirectoryPath ChocolateyPackages { get; private set; }
    public ICollection<DirectoryPath> ToClean { get; private set; }
    public DirectoryPath ConfigsDirectory { get; private set; }
    public DirectoryPath NativeLibsDirectory { get; private set; }

    public BuildDirectories(
        DirectoryPath source,
        DirectoryPath build,
        DirectoryPath tempBuild,
        DirectoryPath publishedWebsites,
        DirectoryPath publishedApplications,
        DirectoryPath publishedLibraries,
        DirectoryPath nugetNuspecDirectory,
        DirectoryPath chocolateyNuspecDirectory,
        DirectoryPath testResults,
        DirectoryPath nuGetPackages,
        DirectoryPath chocolateyPackages,
        DirectoryPath configsDirectory = null,        
        DirectoryPath nativeLibsDirectory = null        
        )
    {
        Source = source;
        Build = build;
        TempBuild = tempBuild;
        PublishedWebsites = publishedWebsites;
        PublishedApplications = publishedApplications;
        PublishedLibraries = publishedLibraries;
        NugetNuspecDirectory = nugetNuspecDirectory;
        ChocolateyNuspecDirectory = chocolateyNuspecDirectory;
        TestResults = testResults;
        NuGetPackages = nuGetPackages;
        ChocolateyPackages = chocolateyPackages;
        ConfigsDirectory = configsDirectory;
        NativeLibsDirectory = nativeLibsDirectory;

        ToClean = new[] {
            Build,
            TempBuild,
            TestResults,
            NuGetPackages
        };
    }
}