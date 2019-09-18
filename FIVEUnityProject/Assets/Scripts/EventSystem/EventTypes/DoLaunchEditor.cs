using System;

namespace FIVE.EventSystem
{
    public abstract class DoLaunchEditor : IEventType<LauncherEditorArgs> { }

    public sealed class LauncherEditorArgs : EventArgs
    {
        public string Code;
        public bool Saved = false;
    }
}