using FIVE.EventSystem;
using FIVE.Robot;
using System;

namespace FIVE.UI.CodeEditor
{
    public class UpdateScriptEventArgs : EventArgs
    {
        public string Code { get; }

        public UpdateScriptEventArgs(string code)
        {
            Code = code;
        }
    }

    public class OnCodeEditorSaved : IEventType<UpdateScriptEventArgs>
    {
    }
}