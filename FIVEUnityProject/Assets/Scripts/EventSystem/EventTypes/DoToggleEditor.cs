using System;

namespace FIVE.EventSystem
{
    public abstract class DoToggleEditor : IEventType<LauncherEditorArgs> { }

    public sealed class LauncherEditorArgs : EventArgs { }
}