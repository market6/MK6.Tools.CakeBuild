using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace MK6.Tools.CakeBuild.Chef
{
    public class KnifeRunner : Tool<KnifeSettings>
    {
        private static string[] _executableNames = new[] { "knife", "knife.bat" };
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;
        private readonly IProcessRunner _processRunner;

        public KnifeRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools) : base(fileSystem, environment, processRunner, tools)
        {
            _fileSystem = fileSystem;
            _environment = environment;
            _processRunner = processRunner;
        }
        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return _executableNames;
        }

        protected override IEnumerable<FilePath> GetAlternativeToolPaths(KnifeSettings settings)
        {
            var toolPaths = new List<FilePath>(base.GetAlternativeToolPaths(settings));
            var chefdkInstalled = false;

            const string defaultPathWin = @"C:\opscode\chefdk\bin";
            const string defaultPathNix = "/usr/bin";

            //Look for chefdk installed on the system
            var defaultPath = _environment.Platform.IsUnix() ? _fileSystem.GetDirectory(defaultPathNix) : _fileSystem.GetDirectory(defaultPathWin);
            var knifeExec = _environment.Platform.IsUnix() ? _fileSystem.GetFile(_executableNames.Single(x => !x.EndsWith(".bat"))) : _fileSystem.GetFile(_executableNames.Single(x => x.EndsWith(".bat")));

            var knifePath = defaultPath.Path.CombineWithFilePath(knifeExec.Path);
            
            if (_fileSystem.Exist(knifePath))
                chefdkInstalled = true;

            if (!String.IsNullOrWhiteSpace(_environment.GetEnvironmentVariable("CHEF_DK_INSTALL_PATH")))
            {
                knifePath = _environment.GetEnvironmentVariable("CHEF_DK_INSTALL_PATH");
                if (_fileSystem.Exist(knifePath))
                    chefdkInstalled = true;
            }

            if (chefdkInstalled)
            {
                toolPaths.Add(knifePath);
                return toolPaths;
            }

            if (_environment.Platform.Family != PlatformFamily.Windows)
                throw new NotImplementedException("Knife executable not found--automatic install of chef dk is currently only supported on windows.");

            //Install chefdk on the system if it isn't found
            var installerFileName = $"chefdk-{settings.ChefDKVersion}-1-x86.msi";
            var packageUri = $"https://packages.chef.io/stable/windows/2012r2/{installerFileName}";
            byte[] installerBytes = null;
            using (var httpClient = new HttpClient())
            {
                installerBytes = httpClient.GetByteArrayAsync(packageUri).Result;
            }

            var installerPath = _fileSystem
                .GetFile(_environment.GetSpecialPath(SpecialPath.LocalTemp)
                .GetFilePath(installerFileName));

            installerPath
                .OpenWrite()
                .Write(installerBytes, 0, installerBytes.Length);

            _processRunner
                .Start(installerPath.Path, new ProcessSettings { Arguments = new ProcessArgumentBuilder().Append("/qn /quiet /norestart") })
                .WaitForExit();

            toolPaths.Add(knifePath);
            return toolPaths;
        }

        protected override string GetToolName()
        {
            return "knife";
        }

        public void Run(KnifeSettings settings)
        {
            var args = BuildArguments(settings);
            Run(settings, args);
        }

        private static ProcessArgumentBuilder BuildArguments(KnifeSettings settings)
        {
            var builder = new ProcessArgumentBuilder();
            foreach (var kvp in settings.Options.Where(x => x.Key.StartsWith("--")))
            {
                builder.AppendSwitch(kvp.Key, kvp.Value);
            }

            return builder;
        }
    }
}
