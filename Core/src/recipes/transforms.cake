Task("Transform")
  .Description("")
  .Does(() =>
{
  EnsureDirectoryExists(parameters.Paths.Directories.ConfigsDirectory);
  CleanDirectories(parameters.Paths.Directories.ConfigsDirectory.FullPath);
  
  var appConfigGlobPattern = parameters.Paths.Directories.TempBuild.FullPath + "/App.*.Config";
  Information("Performing transforms on app configs using glob pattern: {0}", appConfigGlobPattern);
  foreach(var file in GetFiles(appConfigGlobPattern))
  {
      Information("Transform: {0}", file);
      var possibleExecutables = GetFiles(parameters.Paths.Directories.TempBuild.FullPath + "/*.exe").ToList();
      if(possibleExecutables.Count == 1)
        TransormConfigs(Context, file, parameters.Paths.Directories.ConfigsDirectory, possibleExecutables[0]);
      else
        Error("Error during transform. Unable to determine the executable path: {0} found in TempBuild folder {1}", possibleExecutables.Count == 0 ? "No executables" : "Multiple executables", parameters.Paths.Directories.TempBuild.FullPath);
  }

  var webConfigGlobPattern = parameters.Paths.Directories.PublishedWebsites.FullPath + "/**/Web.*.Config";
  Information("Performing transforms on web configs using glob pattern: {0}", webConfigGlobPattern);
  
  //Transform release config first--use the result as the source for the env transforms
  var releaseConfigGlob = parameters.Paths.Directories.PublishedWebsites.FullPath + "/**/Web.Release.config";
  Information("Looking for release config using glob pattern: {0}", releaseConfigGlob);

  var releaseConfig = GetFiles(releaseConfigGlob).FirstOrDefault();
  if(releaseConfig != null)
  {
    Information("Transforming Release Config");
    var sourceFile = releaseConfig.GetDirectory().CombineWithFilePath("web.config");
    var targetFile = releaseConfig.GetDirectory().CombineWithFilePath("web.config.tmp");
    XdtTransformConfig(sourceFile, releaseConfig, targetFile);
    DeleteFile(sourceFile);
    MoveFile(targetFile, sourceFile);
  }
  else
    Warning("Skipping release transform...release config not found!");
    
  foreach(var file in GetFiles(webConfigGlobPattern))
  {
      if(file.FullPath.EndsWith("Web.Release.config", StringComparison.OrdinalIgnoreCase))
      {
        Verbose("Skipping release config: {0}", file.FullPath);
        continue;
      }

      Information("Transform: {0}", file);
      TransormConfigs(Context, file, parameters.Paths.Directories.ConfigsDirectory);
  }

});

public static void TransormConfigs(ICakeContext context, FilePath fileTransform, DirectoryPath configsDirectory, FilePath executablePath = null)
{
  var isWebApp = executablePath == null ? true : false;
  var configPrefix = isWebApp ? "web" : "app";
  var env = fileTransform.GetFilename().ToString().ToLower().Replace(configPrefix + ".", "").Replace(".config", "");
  var sourceFile = string.Format("{0}/{1}.config", fileTransform.GetDirectory().FullPath, configPrefix);
  var envDirectory = configsDirectory.Combine(context.Directory(env));
  context.EnsureDirectoryExists(envDirectory);
  string targetFile;
  if(!isWebApp)
    targetFile = envDirectory.CombineWithFilePath(executablePath.GetFilename()).FullPath + ".config";
  else
    targetFile = envDirectory.CombineWithFilePath(configPrefix + ".config").FullPath;
  
  context.Verbose("targetFile: {0}", targetFile);
  context.XdtTransformConfig(sourceFile, fileTransform, targetFile);
}