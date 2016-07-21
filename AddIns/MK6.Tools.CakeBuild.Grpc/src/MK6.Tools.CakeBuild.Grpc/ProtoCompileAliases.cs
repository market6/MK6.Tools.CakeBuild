using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace MK6.Tools.CakeBuild.Grpc
{
    public static class ProtoCompileAliases
    {
        [CakeMethodAlias]
        public static void ProtoCompile(this ICakeContext context, ProtoCompileSettings settings)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var resolver = new ProtoCompileToolResolver(context.FileSystem, context.Environment, context.Tools);
            var runner = new ProtoCompileRunner(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools, resolver);

            runner.Compile(settings);
        }

        [CakeMethodAlias]
        public static void ProtoCompile(this ICakeContext context, FilePath protoInputFile)
        {
            context.ProtoCompile(new ProtoCompileSettings { ProtoInputFile = protoInputFile});
        }
    }
}
