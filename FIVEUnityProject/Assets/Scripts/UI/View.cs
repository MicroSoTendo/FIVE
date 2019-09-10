using UnityEngine;

namespace FIVE.UI
{
    public abstract class View
    {
        public RenderMode RenderMode { get; set; }

        public static void Create<T>() where T : View
        {
            var fileName = typeof(T).Name + ".xml";
            var xmlFile = Resources.Load<TextAsset>($"UI/{fileName}");
        }
        protected View()
        {
            
        }
    }
}