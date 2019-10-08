using System;
using FIVE.EventSystem;

namespace FIVE.UI.CodeEditor
{
    public class CodeEditorSavedEventArgs : EventArgs
    {
        public string Code { get; }
        public CodeEditorSavedEventArgs(string code) => Code = code;
    }
    public class OnCodeEditorSaved : IEventType<CodeEditorSavedEventArgs> { }
}
