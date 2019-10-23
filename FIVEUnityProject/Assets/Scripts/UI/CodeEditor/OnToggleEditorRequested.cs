using FIVE.EventSystem;
using FIVE.Robot;
using System;

namespace FIVE.UI.CodeEditor
{
    public abstract class OnToggleEditorRequested : IEventType<LauncherEditorArgs>
    {
    }

    public sealed class LauncherEditorArgs : EventArgs
    {
        public RobotSphere Target;
    }
}