using System;
using FIVE.EventSystem;
namespace FIVE.UI.AWSLEditor
{
    internal class AWSLEditorViewModel : ViewModel<AWSLEditorView, AWSLEditorViewModel>
    {
        public string CodeText => View.CodeInputField.text;

        public AWSLEditorViewModel()
        {
            binder.Bind(v => v.SaveButton.onClick).To(vm => vm.OnSaveButtonClicked);
            binder.Bind(v => v.CancelButton.onClick).To(vm => vm.OnCancelButtonClicked);
            EventManager.Subscribe<DoToggleEditor>(ToggleEditor);
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
