using System;
using System.Collections.Generic;
using System.Reflection;

namespace FIVE.FIVE.Network
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RPCAttribute : Attribute
    {
        public static HashSet<Assembly> ValidAssemblies { get; } = new HashSet<Assembly>();
    }
}
