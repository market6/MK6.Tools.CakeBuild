using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core.IO;
using MK6.Common.Messaging.Pipes;

namespace MK6.Tools.CakeBuild.NamedPipes
{
    public class NamedPipeResult
    {
        public INamedPipeClient PipeClient { get; set; }
        public IProcess Process { get; set; }
        public NamedPipeSettings PipeSettings { get; set; }
        public IEnumerable<string> LogMessages { get; set; }
    }
}
