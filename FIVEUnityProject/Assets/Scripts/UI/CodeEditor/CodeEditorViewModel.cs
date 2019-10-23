using System.Linq;
using System.Text;
using TMPro;
using UnityEngine.UI;
using static FIVE.Util;

namespace FIVE.UI.CodeEditor
{
    internal class CodeEditorViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/CodeEditor/CodeEditor";
        public TMP_InputField CodeInputField { get; }
        public TMP_InputField LineNumber { get; }
        public Button SaveButton { get; }
        public Button RunButton { get; }
        public Button PauseButton { get; }
        public Button StopButton { get; }
        public Button ExitButton { get; }

        public bool IsFocused => CodeInputField.isFocused;

        public CodeEditorViewModel()
        {
            CodeInputField = Get<TMP_InputField>(nameof(CodeInputField));
            LineNumber = Get<TMP_InputField>(nameof(LineNumber));
            SaveButton = Get<Button>(nameof(SaveButton));
            RunButton = Get<Button>(nameof(RunButton));
            PauseButton = Get<Button>(nameof(PauseButton));
            StopButton = Get<Button>(nameof(StopButton));
            ExitButton = Get<Button>(nameof(ExitButton));

            UpdateLineNumber("");
            Bind(SaveButton).To(RunButtonClicked);
            Bind(ExitButton).To(OnCancelButted);
            Subscribe<OnToggleEditorRequested>(ToggleEditor);
            CodeInputField.onValueChanged.AddListener(UpdateLineNumber);
        }

        private void UpdateLineNumber(string text)
        {
            int numberOfLines = text.Count(c => c == '\n');
            var sb = new StringBuilder();
            for (int i = 1; i < numberOfLines + 2; i++)
            {
                sb.AppendLine(i.ToString());
            }

            LineNumber.text = sb.ToString();
        }

        private void ToggleEditor()
        {
            ToggleEnabled();
            UIManager.SetCursor(IsActive ? UIManager.CursorType.Regular : UIManager.CursorType.Aim);
        }

        private void RunButtonClicked()
        {
            this.RaiseEvent<OnCodeEditorSaved, CodeEditorSavedEventArgs>(
                new CodeEditorSavedEventArgs(CodeInputField.text));
            IsActive = false;
            UIManager.SetCursor(UIManager.CursorType.Aim);
        }

        private void OnCancelButted()
        {
            IsActive = false;
            UIManager.SetCursor(UIManager.CursorType.Aim);
        }
    }
}