using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cake.Core.IO;
using Cake.Core.Tooling;
using MK6.Common.Messaging.Pipes;

namespace MK6.Tools.CakeBuild.NamedPipes
{
    public class NamedPipeSettings : ToolSettings
    {
        public NamedPipeSettings()
        {
            PipeRemoteServerName = ".";
            CancellationTokenSource = new CancellationTokenSource();
            TypeMapper = TypeMap.CreateDefaultMapper();
        }

        public IDictionary<string, string> PipeServerArguments { get; set; }
        public string PipeName { get; set; }
        public string PipeRemoteServerName { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public IPipeMessageSerializer PipeMessageSerializer { get; set; }
        public IDictionary<byte, TypeMap> TypeMapper { get; set; }
    }
}
