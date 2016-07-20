using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;
using NSubstitute;

namespace MK6.Tools.CakeBuild.Grpc.Tests
{
    public class ProtoCompileToolResolverFixture
    {
        public ICakeEnvironment Environment { get; set; }
        public IFileSystem FileSystem { get; set; }
        public IToolLocator ToolLocator { get; set; }

        public ProtoCompileToolResolverFixture(PlatformFamily family, bool is64Bit)
        {
            Environment = Substitute.For<ICakeEnvironment>();
            Environment.Platform.Returns(info =>
            {
                var platform = Substitute.For<ICakePlatform>();
                platform.Is64Bit.Returns(is64Bit);
                platform.Family.Returns(family);
                return platform;
            });

            FileSystem = Substitute.For<IFileSystem>();
            ToolLocator = Substitute.For<IToolLocator>();
        }
    }
}