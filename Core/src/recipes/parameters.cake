public class BuildParameters
{
    public string Target { get; private set; }
    public string Configuration { get; private set; }
    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnUnix { get; private set; }
    public bool IsRunningOnWindows { get; private set; }
    // public bool IsPullRequest { get; private set; }
    // public bool IsMainRepository { get; private set; }
    public bool IsMasterBranch { get; private set; }
    public bool IsTagged { get; private set; }
    public bool IsPublishBuild { get; private set; }
    public bool IsReleaseBuild { get; private set; }
    public SlackCredentials Slack { get; private set; }
    public NuGetCredentials NuGet { get; private set; }
    public BuildVersion Version { get; private set; }
    public BuildPaths Paths { get; private set; }
    public FilePath BuildConfigFilePath { get; private set; }
    

    public void SetBuildVersion(BuildVersion version)
    {
        Version  = version;
    }

    public void SetBuildPaths(BuildPaths paths)
    {
        Paths  = paths;
    }

    public static BuildParameters GetParameters(
        ICakeContext context,
        BuildSystem buildSystem
        //string repositoryOwner,
        //string repositoryName
        )
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var target = context.Argument("target", "Default");
        var configuration = context.Argument("configuration", "Release");
        var configPath = context.Argument("buildConfig", "build.json");

        return new BuildParameters {
            Target = target,
            Configuration = configuration,
            BuildConfigFilePath = configPath,
            IsLocalBuild = buildSystem.IsLocalBuild,
            IsRunningOnUnix = context.IsRunningOnUnix(),
            IsRunningOnWindows = context.IsRunningOnWindows(),
            IsMasterBranch = buildSystem.IsLocalBuild ? false : StringComparer.OrdinalIgnoreCase.Equals("refs/heads/master", context.EnvironmentVariable("teamcity.build.vcs.branch.Mk6BitbucketCommon")),
            Slack = GetSlackCredentials(context),
            NuGet = GetNuGetCredentials(context),
            IsPublishBuild = new [] {
                "Create-Release-Notes"
            }.Any(
                releaseTarget => StringComparer.OrdinalIgnoreCase.Equals(releaseTarget, target)
            ),
            IsReleaseBuild = new [] {
                "Publish-NuGet-Packages",
                "Publish-Chocolatey-Packages",
                "Publish-GitHub-Release"
            }.Any(
                publishTarget => StringComparer.OrdinalIgnoreCase.Equals(publishTarget, target)
            )
        };
    }
}