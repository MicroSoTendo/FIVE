using System;
using FIVE.EventSystem;

namespace FIVE.UI.CodeEditor
{
    public abstract class OnToggleEditorRequested : IEventType<LauncherEditorArgs> { }

    public sealed class LauncherEditorArgs : EventArgs { }
}