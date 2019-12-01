using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FIVE.Network.Rpc
{
    internal class RpcManager : MonoBehaviour
    {
        public BijectMap<int, MethodInfo> RpcInfos { get; internal set; }
        private class MethodComparer : IComparer<MethodInfo>
        {
            public int Compare(MethodInfo x, MethodInfo y)
            {
                return x == null ? 1 : x.Name.CompareTo(y);
            }
        }
        private IEnumerator Start()
        {
            var methodsInfos = new SortedSet<MethodInfo>(new MethodComparer());
            foreach (Assembly assembly in RpcAttribute.ValidAssemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IRpcInvokeable).IsAssignableFrom(type))
                    {
                        foreach (MethodInfo methodInfo in type.GetMethods())
                        {
                            if (methodInfo.GetCustomAttributes(typeof(RpcAttribute), false).Length > 0)
                            {
                                methodsInfos.Add(methodInfo);
                            }
                        }
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
            if (methodsInfos.Count > 0)
            {
                RpcInfos = new BijectMap<int, MethodInfo>();
                int counter = 0;
                foreach (MethodInfo methodsInfo in methodsInfos)
                {
                    RpcInfos.Add(counter++, methodsInfo);
                    yield return new WaitForFixedUpdate();
                }
            }

        }
    }
}
