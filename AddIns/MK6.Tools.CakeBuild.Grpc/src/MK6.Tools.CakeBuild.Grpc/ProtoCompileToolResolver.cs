using System.Linq;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace MK6.Tools.CakeBuild.Grpc
{
    public class ProtoCompileToolResolver : IProtoCompileToolResolver
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;
        private readonly IToolLocator _tools;

        public ProtoCompileToolResolver(IFileSystem fileSystem, ICakeEnvironment environment, IToolLocator tools)
        {
            _fileSystem = fileSystem;
            _environment = environment;
            _tools = tools;
        }

        /// <summary>
        /// Resolves the path to the protoc executable for the environment.
        /// </summary>
        /// <returns>The path to protoc</returns>
        public FilePath ResolveProtocPath()
        {
            var rootPath = ResolveRootPath();
            var toolPath = rootPath.CombineWithFilePath(_environment.Platform.IsUnix() ? "protoc" : "protoc.exe");

            return toolPath;
        }

        /// <summary>
        /// Resolves the path to the grpc_csharp_plugin executable for the environment.
        /// </summary>
        /// <returns>The path to grpc_csharp_plugin</returns>
        public FilePath ResolveGrpcCSharpPluginPath()
        {
            var rootPath = ResolveRootPath();
            var toolPath = rootPath.CombineWithFilePath(_environment.Platform.IsUnix() ? "grpc_csharp_plugin" : "grpc_csharp_plugin.exe");

            return toolPath;
        }

        private DirectoryPath ResolveRootPath()
        {
            var platform = _environment.Platform.IsUnix() ? "linux_" : "windows_";
            platform = _environment.Platform.Is64Bit ? platform + "x64" : platform + "x86";
            var protocPath = _tools.Resolve(_environment.Platform.IsUnix() ? $"{platform}/protoc" : $"{platform}/protoc.exe");
            
            var toolPath = protocPath.GetDirectory();
            if (toolPath == null || !_fileSystem.Exist(toolPath))
                throw new CakeException($"Could not locate the grpc.tools folder for the platform {platform}");

            return toolPath;
        }
    }
}