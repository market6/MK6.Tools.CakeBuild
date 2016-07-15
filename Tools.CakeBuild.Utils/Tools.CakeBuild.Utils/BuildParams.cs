using System;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;

namespace Tools.CakeBuild.Utils
{
    public class BuildParams
    {
        private const string SolutionPathKey = "solutionPath";
        private const string TargetKey = "target";
        private const string ConfigurationKey = "configuration";
        private const string VersionKey = "appVer";

        public string SolutionPath { get; private set; }
        public string Target { get; private set; }
        public string Configuration { get; private set; }
        public string Version { get; private set; }

        public static BuildParams GetParams(ICakeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string solutionPath;
            if (!context.Arguments.HasArgument(SolutionPathKey))
            {
                //Look for a sln file in the build root
                var slnFiles = context.Globber.GetFiles(".sln").ToArray();
                switch (slnFiles.Length)
                {
                    case 0:
                        throw new Exception("No .sln files found in the source root. Please specify the argument \"" + SolutionPathKey + "\"");
                    case 1:
                        solutionPath = slnFiles[0].FullPath;
                        break;
                    default:
                        throw new Exception("More than 1 .sln file found in the source root. Please specify the argument \"" + SolutionPathKey + "\"");
                }
            }
            else
                solutionPath = context.Arguments.GetArgument(SolutionPathKey);


            return new BuildParams
            {
                SolutionPath = solutionPath,
                Target = context.Arguments.GetArgument(TargetKey) ?? "Default",
                Configuration = context.Arguments.GetArgument(ConfigurationKey) ?? "Release",
                Version = context.Arguments.GetArgument(VersionKey) ?? "1.0.0"
            };
        }

    }
}
