using System;
using System.Collections.Generic;
using System.Reflection;

namespace FIVE.Network.Rpc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcAttribute : Attribute
    {
        public static HashSet<Assembly> ValidAssemblies { get; } = new HashSet<Assembly>();
    }
}
