using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;

namespace MK6.Tools.CakeBuild.NamedPipes
{
    public static class NamedPipeAlias
    {
        [CakeMethodAlias]
        public static NamedPipeRunner NamedPipeRunner(this ICakeContext context)
        {
            return new NamedPipeRunner(context);
        }

        [CakeMethodAlias]
        public static NamedPipeResult NamedPipeRunnerRun(this ICakeContext context, NamedPipeSettings settings, Action<NamedPipeResult> runAction)
        {
            System.Diagnostics.Debugger.Launch();
            var runner = new NamedPipeRunner(context);
            var result = runner.Run(settings);
            runAction(result);
            context.Log.Information("runner.EndRun");
            runner.EndRun(result);
            return result;
        }
    }
}
