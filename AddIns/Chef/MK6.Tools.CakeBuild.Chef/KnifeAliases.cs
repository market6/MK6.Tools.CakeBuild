using Cake.Core;
using Cake.Core.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MK6.Tools.CakeBuild.Chef
{
    public static class KnifeAliases
    {
        [CakeMethodAlias]
        public static void KnifeBootstrap(this ICakeContext context, KnifeSettings settings)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var runner = new KnifeRunner(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools);
            
        }
    }
}
