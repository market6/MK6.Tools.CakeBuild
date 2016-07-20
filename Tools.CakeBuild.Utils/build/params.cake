


public class BuildParams
{
  private const string solutionPathKey = "solutionPath";
  private const string targetKey = "target";
  private const string configurationKey = "configuration";
  private const string versionKey = "appVer";

  public string SolutionPath { get; private set; }
  public string Target { get; private set; }
  public string Configuration { get; private set; }
  public string Version { get; private set; }

  public static BuildParams GetParams(ICakeContext context)
  {
    if (context == null)
    {
        throw new ArgumentNullException("context");
    }

    string solutionPath;
    if(!context.HasArgument(solutionPathKey))
    {
      context.Information("Attempting to load default .sln file...");
      var slnFiles = context.Globber.GetFiles("*.sln").ToArray();
      switch(slnFiles.Length)
      {
        case 0:
          context.Information(Environment.CurrentDirectory);
          throw new Exception("No .sln files found in the source root. Please specify the argument \"" + solutionPathKey + "\"");
          return null;
        case 1:
          solutionPath = slnFiles[0].FullPath;
          context.Information("Found " + solutionPath);
          break;
        default:
          throw new Exception("More than 1 .sln file found in the source root. Please specify the argument \"" + solutionPathKey + "\"");
          return null;
      }
    }
    else
      solutionPath = context.Argument<string>(solutionPathKey);


    return new BuildParams {
      SolutionPath = solutionPath,
      Target = context.Argument(targetKey, "Default"),
      Configuration = context.Argument(configurationKey, "Release"),
      Version = context.Argument(versionKey, "1.0.0")
    };
  }
  
}