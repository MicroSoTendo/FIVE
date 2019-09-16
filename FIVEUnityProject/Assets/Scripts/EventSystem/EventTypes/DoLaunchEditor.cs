using System;

namespace FIVE.EventSystem
{
    public abstract class DoLaunchEditor : IEventType<EventHandler<LauncherEditorArgs>, LauncherEditorArgs> { }

    public sealed class LauncherEditorArgs : EventArgs
    {
        public string Code;
    }
}