using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.IO.Arguments;
using Cake.Core.Tooling;

namespace MK6.Tools.CakeBuild.Grpc
{
    public class ProtoCompileRunner : Tool<ProtoCompileSettings>
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;
        private readonly IProtoCompileToolResolver _toolResolver;

        public ProtoCompileRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools, IProtoCompileToolResolver toolResolver) : base(fileSystem, environment, processRunner, tools)
        {
            _fileSystem = fileSystem;
            _environment = environment;
            _toolResolver = toolResolver;
        }

        protected override string GetToolName()
        {
            return Constants.ToolName;
        }

        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return Constants.ToolExecutableNames;
        }

        protected override IEnumerable<FilePath> GetAlternativeToolPaths(ProtoCompileSettings settings)
        {
            var toolPath = _toolResolver.ResolveProtocPath();

            return new[] { toolPath };
        }

        public void Compile(ProtoCompileSettings settings)
        {
            if(settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.ProtoInputFile == null || !_fileSystem.Exist(settings.ProtoInputFile))
            {
                throw new CakeException("ProtoInputFile not found");    
            }

            if (settings.CSharpOutputPath != null)
            {
                var dir = _fileSystem.GetDirectory(settings.CSharpOutputPath);
                if(!dir.Exists)
                    dir.Create();
            }
            Run(settings, GetArguments(settings));
        }

        private ProcessArgumentBuilder GetArguments(ProtoCompileSettings settings)
        {

            var argBuilder = new ProcessArgumentBuilder();
            if(settings.ProtoImportPath != null)
                argBuilder.Append("--proto_path={0}", settings.ProtoImportPath.MakeAbsolute(_environment));
            else//use the folder where the .proto file is
                argBuilder.Append("--proto_path={0}", settings.ProtoInputFile.GetDirectory().MakeAbsolute(_environment));

            argBuilder.Append("--csharp_out {0}", settings.CSharpOutputPath ?? _environment.WorkingDirectory);
            argBuilder.Append("--grpc_out {0}", settings.CSharpOutputPath ?? _environment.WorkingDirectory);
            argBuilder.Append("--plugin=protoc-gen-grpc={0}", _toolResolver.ResolveGrpcCSharpPluginPath());
            argBuilder.Append(settings.ProtoInputFile.MakeAbsolute(_environment).FullPath);

            return argBuilder;
        }
    }
}