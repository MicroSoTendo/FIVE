using FIVE.EventSystem;
using FIVE.Robot;
using System;

namespace FIVE.UI.CodeEditor
{
    public class UpdateScriptEventArgs : EventArgs
    {
        public RobotSphere Target;
        public string Code { get; }

        public UpdateScriptEventArgs(RobotSphere target, string code)
        {
            Target = target;
            Code = code;
        }
    }

    public class OnCodeEditorSaved : IEventType<UpdateScriptEventArgs>
    {
    }
}