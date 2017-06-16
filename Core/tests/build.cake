Task("VerifyLPM")
  .Does(() =>
{
  var fileSystemType = Context.FileSystem.GetType();

  if (fileSystemType.ToString()=="Cake.LongPath.Module.LongPathFileSystem")
  {
      Information("Sucessfully loaded {0}", fileSystemType.Assembly.Location);
  }
  else
  {
      Error("Failed to load Cake.LongPath.Module");
  }

});

#l ..\src\recipes\addins.cake
#l ..\src\recipes\tools.cake

#l ..\src\recipes\variables.cake
#l ..\src\recipes\credentials.cake
#l ..\src\recipes\gitversion.cake
#l ..\src\recipes\nuget.cake
#l ..\src\recipes\packages.cake
#l ..\src\recipes\parameters.cake
#l ..\src\recipes\paths.cake
#l ..\src\recipes\slack.cake
#l ..\src\recipes\testing.cake
#l ..\src\recipes\octopus.cake
#l ..\src\recipes\transforms.cake
#l ..\src\recipes\config.cake

#l ..\src\recipes\zbuild.cake