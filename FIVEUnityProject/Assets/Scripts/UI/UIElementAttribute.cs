using System;

namespace FIVE.UI
{
    [AttributeUsage(validOn: AttributeTargets.Property | AttributeTargets.Field)]
    public class UIElementAttribute : Attribute
    {
        public string Path { get; }
        public UIElementAttribute(string path = null)
        {
            Path = path;
        }
    }
}
