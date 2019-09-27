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
            EventManager.Subscribe<DoLaunchEditor>(LaunchEditor);
        }

        private void LaunchEditor(object sender, EventArgs e)
        {
            ToggleEnabled();
        }

        public void OnSaveButtonClicked(object sender, EventArgs e)
        {
            this.RaiseEvent<OnCodeEditorSaved, CodeEditorSavedEventArgs>(new CodeEditorSavedEventArgs(CodeText));
        }

        public void OnCancelButtonClicked(object sender, EventArgs e)
        {
            SetEnabled(false);
        }
    }
}
