using System;

namespace FIVE.UI
{
    [AttributeUsage(validOn: AttributeTargets.Property | AttributeTargets.Field)]
    public class UIElementAttribute : Attribute
    {
        public string Path { get; }
        public TargetType TargetType { get; }
        public UIElementAttribute() 
        { 
            Path = null;
            TargetType = TargetType.Default;
        }
        public UIElementAttribute(string path, TargetType targetType)
        {
            Path = path;
            TargetType = targetType;
        }
    }
    public enum TargetType { Default = 0, XML = 1, Property = 2, Field = 3}
}
