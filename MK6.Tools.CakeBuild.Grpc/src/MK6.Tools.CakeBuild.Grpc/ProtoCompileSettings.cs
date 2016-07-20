using System.Collections.Generic;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace MK6.Tools.CakeBuild.Grpc
{
    public class ProtoCompileSettings : ToolSettings
    {
        public DirectoryPath ProtoImportPath { get; set; }
        public DirectoryPath CSharpOutputPath { get; set; }
        public FilePath ProtoInputFile { get; set; }

        
    }
}