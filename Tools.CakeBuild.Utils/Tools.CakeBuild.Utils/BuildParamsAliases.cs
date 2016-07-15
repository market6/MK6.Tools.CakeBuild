using System;
using Cake.Core;
using Cake.Core.Annotations;

namespace Tools.CakeBuild.Utils
{

    public static class BuildParamsAliases
    {
        [CakeMethodAlias]
        public static BuildParams BuildParams(this ICakeContext context)
        {
            
            return Utils.BuildParams.GetParams(context);
        }
    }
}
