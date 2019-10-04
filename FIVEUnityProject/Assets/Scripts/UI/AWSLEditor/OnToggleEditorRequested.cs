using System;
using FIVE.EventSystem;

namespace FIVE.UI.AWSLEditor
{
    public abstract class OnToggleEditorRequested : IEventType<LauncherEditorArgs> { }

    public sealed class LauncherEditorArgs : EventArgs { }
}