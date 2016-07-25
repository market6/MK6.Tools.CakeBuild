


public class BuildParams
{
  private const string solutionPathKey = "solutionPath";
  private const string targetKey = "target";
  private const string configurationKey = "configuration";
  private const string versionKey = "appVer";
  private const string companyKey = "company";

  public string SolutionPath { get; set; }
  public string Target { get; set; }
  public string Configuration { get; set; }
  public string Version { get; set; }
  public string Company { get; set; }

  public static BuildParams GetParams(ICakeContext context)
  {
    if (context == null)
    {
        throw new ArgumentNullException("context");
    }

    string solutionPath = null;
    if(!context.HasArgument(solutionPathKey))
    {
      context.Information("Attempting to load default .sln file...");
      var slnFiles = context.Globber.GetFiles("*.sln").ToArray();
      switch(slnFiles.Length)
      {
        case 0:
          context.Information("No .sln files found in the source root.");
          //throw new Exception("No .sln files found in the source root. Please specify the argument \"" + solutionPathKey + "\"");
          //return null;
          break;
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