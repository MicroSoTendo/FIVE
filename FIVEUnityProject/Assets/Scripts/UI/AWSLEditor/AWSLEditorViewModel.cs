using System.Text;
using System.Linq;
using FIVE.EventSystem;
using System;

namespace FIVE.UI.AWSLEditor
{
    internal class OnCodeTextChanged : IEventType { }
    internal class AWSLEditorViewModel : ViewModel<AWSLEditorView, AWSLEditorViewModel>
    {
        public string CodeText => View.CodeText.text;
        public bool IsFocused => View.CodeInputField.isFocused;
        public AWSLEditorViewModel()
        {
            UpdateLineNumber(null, null);
            binder.Bind(v => v.SaveButton.onClick).To(vm => vm.OnSaveButtonClicked);
            binder.Bind(v => v.CancelButton.onClick).To(vm => vm.OnCancelButtonClicked);
            EventManager.Subscribe<OnToggleEditorRequested>(ToggleEditor);
            EventManager.Subscribe<OnCodeTextChanged>(UpdateLineNumber);
        }

        private void UpdateLineNumber(object sender, EventArgs e)
        {
            int numberOfLines = CodeText.Count(c => c == '\n');
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < numberOfLines + 2; i++)
            {
                sb.AppendLine(i.ToString());
            }
            View.LineNumber.text = sb.ToString();
        }

        private void ToggleEditor(object sender, EventArgs e)
        {
            ToggleEnabled();
            UIManager.SetCursor(IsEnabled ? UIManager.CursorType.Regular : UIManager.CursorType.Aim);
        }

        public void OnSaveButtonClicked(object sender, EventArgs e)
        {
            this.RaiseEvent<OnCodeEditorSaved, CodeEditorSavedEventArgs>(new CodeEditorSavedEventArgs(CodeText));
            SetEnabled(false);
            UIManager.SetCursor(UIManager.CursorType.Aim);
        }

        public void OnCancelButtonClicked(object sender, EventArgs e)
        {
            SetEnabled(false);
            UIManager.SetCursor(UIManager.CursorType.Aim);
        }
    }
}
