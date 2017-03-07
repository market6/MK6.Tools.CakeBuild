public class SlackCredentials
{
    public string Token { get; private set; }
    public string Channel { get; private set; }

    public SlackCredentials(string token, string channel)
    {
        Token = token;
        Channel = channel;
    }
}

public class NuGetCredentials
{
    public string ApiKey { get; private set; }
    public string SourceUrl { get; private set; }

    public NuGetCredentials(string apiKey, string sourceUrl)
    {
        ApiKey = apiKey;
        SourceUrl = sourceUrl;
    }

}

public static SlackCredentials GetSlackCredentials(ICakeContext context)
{
    return new SlackCredentials(
        context.EnvironmentVariable(slackTokenVariable),
        context.EnvironmentVariable(slackChannelVariable));
}

public static NuGetCredentials GetNuGetCredentials(ICakeContext context)
{
    return new NuGetCredentials(
        context.EnvironmentVariable(nuGetApiKeyVariable),
        context.EnvironmentVariable(nuGetSourceUrlVariable));
}