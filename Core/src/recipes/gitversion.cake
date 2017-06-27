public class BuildVersion
{
    public string Version { get; private set; }
    public string SemVersion { get; private set; }
    public string Milestone { get; private set; }
    public string CakeVersion { get; private set; }
    public bool IsPreRelease { get; private set; }
    public GitVersion GitVersion { get; private set; }

    public static BuildVersion CalculatingSemanticVersion(
        ICakeContext context,
        BuildParameters parameters
        )
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        string version = null;
        string semVersion = null;
        string milestone = null;
        GitVersion gitVersion = null;

        if (context.IsRunningOnWindows())
        {
            context.Information("Calculating Semantic Version...");

            GitVersion assertedVersions = context.GitVersion(new GitVersionSettings
            {
                OutputType = GitVersionOutput.Json,
            });

            version = assertedVersions.MajorMinorPatch;
            semVersion = ReleaseNumber(assertedVersions);
            milestone = string.Concat(version);
            gitVersion = assertedVersions;
            context.Information("Calculated Semantic Version: {0}", semVersion);
        }

        if (string.IsNullOrEmpty(version) || string.IsNullOrEmpty(semVersion))
        {
            context.Information("Fetching version from SolutionInfo...");
            var assemblyInfo = context.ParseAssemblyInfo(parameters.Paths.Files.SolutionInfoFilePath);
            version = assemblyInfo.AssemblyVersion;
            semVersion = assemblyInfo.AssemblyInformationalVersion;
            milestone = string.Concat(version);
        }

        var cakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString();

        return new BuildVersion
        {
            Version = version,
            SemVersion = semVersion,
            Milestone = milestone,
            CakeVersion = cakeVersion,
            GitVersion = gitVersion,
            IsPreRelease = !string.IsNullOrEmpty(gitVersion.PreReleaseLabel)
        };
    }

    public static string ReleaseNumber(GitVersion gitVersion)
    {
        var releaseTag = gitVersion.BranchName == "master"
            ? releaseTag = gitVersion.PreReleaseTag;
            : releaseTag = gitVersion.PreReleaseTagWithDash;

        return string.Format("{0}{1}", gitVersion.MajorMinorPatch, releaseTag);                 
    }
}