#addin "Newtonsoft.Json"
using Newtonsoft.Json;

public class Config
{
  public string SourceDirectoryPath { get; set; }
  public string Title { get; set; }
  public string SolutionFilePath { get; set; }
  public string SolutionDirectoryPath { get; set; }
  public string OctopusProjectName { get; set; }
  public string TestAssemblySearchPattern { get; set; }
  public string NodeJsDirectoryPath { get; set; }
  public OctopusReleaseOptions OctopusReleaseOptions { get; set; }
  

  public static Config BuildJsonConfig(string configPath)
  {
    return JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText(configPath));
  }
}

public class OctopusReleaseOptions
{
    public string ReleaseNumberFormat { get; set; }
    public ICollection<AdditionalPackageFile> AdditionalPackageFiles { get; set; }

}

public class AdditionalPackageFile
{
  public string Source { get; set; }
  public string Target { get; set; }

}

