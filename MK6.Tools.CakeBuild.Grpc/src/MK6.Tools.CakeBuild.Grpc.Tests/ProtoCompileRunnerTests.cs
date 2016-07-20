using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.IO;
using Cake.Core.Tooling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace MK6.Tools.CakeBuild.Grpc.Tests
{
    [TestClass]
    public class ProtoCompileRunnerTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var environment = Substitute.For<ICakeEnvironment>();
            environment.Platform.Returns(info =>
            {
                var platform = Substitute.For<ICakePlatform>();
                platform.Is64Bit.Returns(true);
                platform.Family.Returns(PlatformFamily.Windows);
                return platform;
            });
            
            var fileSystem = Substitute.For<IFileSystem>();
            var processRunner = Substitute.For<IProcessRunner>();
            var globber = new Globber(fileSystem, environment);
            var configuration = Substitute.For<ICakeConfiguration>();

            var toolLocator = new ToolLocator(environment, new ToolRepository(environment), new ToolResolutionStrategy(fileSystem, environment, globber, configuration));
            
        }
    }

}
