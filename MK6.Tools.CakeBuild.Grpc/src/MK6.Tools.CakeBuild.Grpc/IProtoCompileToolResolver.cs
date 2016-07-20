using Cake.Core.IO;

namespace MK6.Tools.CakeBuild.Grpc
{
    public interface IProtoCompileToolResolver
    {
        /// <summary>
        /// Resolves the path to the protoc executable for the environment.
        /// </summary>
        /// <returns>The path to protoc</returns>
        FilePath ResolveProtocPath();

        /// <summary>
        /// Resolves the path to the grpc_csharp_plugin executable for the environment.
        /// </summary>
        /// <returns>The path to grpc_csharp_plugin</returns>
        FilePath ResolveGrpcCSharpPluginPath();

    }
}