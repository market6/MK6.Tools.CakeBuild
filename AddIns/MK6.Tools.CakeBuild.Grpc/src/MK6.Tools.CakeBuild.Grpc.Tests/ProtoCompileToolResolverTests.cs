using System;
using Cake.Core;
using Cake.Core.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace MK6.Tools.CakeBuild.Grpc.Tests
{
    [TestClass]
    public class ProtoCompileToolResolverTests
    {
        [TestMethod]
        public void ResolveProtocPath_Windows64()
        {
            var fixture = new ProtoCompileToolResolverFixture(PlatformFamily.Windows, true);
            var rootPath = new DirectoryPath("c:/tests/tools/grpc.tools/tools/windows_x64");
            fixture.ToolLocator.Resolve("protoc.exe")
                .Returns(rootPath.CombineWithFilePath("protoc.exe"));

            var toolResolver = new ProtoCompileToolResolver(fixture.FileSystem, fixture.Environment, fixture.ToolLocator);

            var protocPath = toolResolver.ResolveProtocPath();

            Assert.AreEqual(rootPath.CombineWithFilePath("protoc.exe").FullPath, protocPath.FullPath);
        }

        [TestMethod]
        public void ResolveGrpcCSharpPluginPath_Windows64()
        {
            var fixture = new ProtoCompileToolResolverFixture(PlatformFamily.Windows, true);
            var rootPath = new DirectoryPath("c:/tests/tools/grpc.tools/tools/windows_x64");
            fixture.ToolLocator.Resolve("protoc.exe")
                .Returns(rootPath.CombineWithFilePath("protoc.exe"));

            var toolResolver = new ProtoCompileToolResolver(fixture.FileSystem, fixture.Environment, fixture.ToolLocator);

            var grpcPath = toolResolver.ResolveGrpcCSharpPluginPath();

            Assert.AreEqual(rootPath.CombineWithFilePath("grpc_csharp_plugin.exe").FullPath, grpcPath.FullPath);
        }
    }
}
