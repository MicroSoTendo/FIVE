using System;

namespace FIVE.EventSystem
{
    public abstract class OnToggleEditorRequested : IEventType<LauncherEditorArgs> { }

    public sealed class LauncherEditorArgs : EventArgs { }
}