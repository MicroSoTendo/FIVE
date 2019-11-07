using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace FIVE.AWSL
{
    [MoonSharpUserData]
    internal class SharedData
    {
        public Dictionary<string, DynValue> data = new Dictionary<string, DynValue>();

        public DynValue this[string k]
        {
            get => data[k];
            set => data[k] = value;
        }
    }
}