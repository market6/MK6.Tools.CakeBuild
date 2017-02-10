using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;
using MK6.Common.Messaging.Pipes;
using Serilog;

namespace MK6.Tools.CakeBuild.NamedPipes
{
    public class NamedPipeRunner : Tool<NamedPipeSettings>
    {
        private readonly ICakeContext _context;

        public NamedPipeRunner(ICakeContext context) : this(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools)
        {
            _context = context;
        }

        public NamedPipeRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools) : base(fileSystem, environment, processRunner, tools)
        {
        }

        /// <summary>
        /// Gets the name of the tool.
        /// </summary>
        /// <returns>
        /// The name of the tool.
        /// </returns>
        protected override string GetToolName()
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the possible names of the tool executable.
        /// </summary>
        /// <returns>
        /// The tool executable name.
        /// </returns>
        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return Enumerable.Empty<string>();
        }

        public async Task<NamedPipeResult> RunAsync(NamedPipeSettings namedPipeSettings)
        {
            var log = Log.Logger; //Create a bridge between ICakeLog and serilog's ILogger
            var p = RunProcess(namedPipeSettings, BuildArguments(namedPipeSettings));
            var pipeClient = new NamedPipeClient(NamedPipeClientStreamFactory.Create(namedPipeSettings.PipeName, namedPipeSettings.PipeRemoteServerName), namedPipeSettings.CancellationTokenSource.Token, namedPipeSettings.PipeMessageSerializer, namedPipeSettings.TypeMapper, log);
            await pipeClient.ConnectAsync().ConfigureAwait(false);
            
            return new NamedPipeResult {PipeClient = pipeClient, Process = p, PipeSettings = namedPipeSettings};
        }

        public NamedPipeResult Run(NamedPipeSettings namedPipeSettings)
        {
            var log = Log.Logger; //Create a bridge between ICakeLog and serilog's ILogger
            var p = RunProcess(namedPipeSettings, BuildArguments(namedPipeSettings));
            var pipeClient = new NamedPipeClient(NamedPipeClientStreamFactory.Create(namedPipeSettings.PipeName, namedPipeSettings.PipeRemoteServerName), namedPipeSettings.CancellationTokenSource.Token, namedPipeSettings.PipeMessageSerializer, namedPipeSettings.TypeMapper, log);
            pipeClient.ConnectAsync().Wait();
            _context?.Log.Information("pipeClient connected");
            return new NamedPipeResult { PipeClient = pipeClient, Process = p, PipeSettings = namedPipeSettings };
        }

        public int EndRun(NamedPipeResult result)
        {
            var logs = new List<string>(result.Process.GetStandardOutput());
            logs.AddRange(result.Process.GetStandardError());

            result.LogMessages = logs;
            //if (result.PipeSettings.ToolTimeout?.Milliseconds == 0)
                result.Process.WaitForExit(1000);
            //else
            //    result.Process.WaitForExit(result.PipeSettings.ToolTimeout?.Milliseconds ?? 1000);

            result.PipeSettings.CancellationTokenSource.Cancel();
            result.PipeClient.Dispose();
            return result.Process.GetExitCode();
        }

        private static ProcessArgumentBuilder BuildArguments(NamedPipeSettings settings)
        {
            var builder = new ProcessArgumentBuilder();
            foreach (var kvp in settings.PipeServerArguments)
            {
                builder.AppendSwitch(kvp.Key, kvp.Value);
            }

            return builder;
        }
    }
}
