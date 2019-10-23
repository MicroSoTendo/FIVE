using FIVE.EventSystem;
using FIVE.Robot;
using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine.UI;

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

        private RobotSphere target;

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
            Bind(SaveButton).To(OnRunButtonClicked);
            Bind(ExitButton).To(OnCancelButtonClicked);
            EventManager.Subscribe<OnToggleEditorRequested>(ToggleEditor);
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

        private void ToggleEditor(object sender, EventArgs args)
        {
            ToggleEnabled();
            UIManager.SetCursor(IsActive ? UIManager.CursorType.Regular : UIManager.CursorType.Aim);
            target = (args as LauncherEditorArgs).Target;
        }

        private void OnRunButtonClicked()
        {
            IsActive = false;
            UIManager.SetCursor(UIManager.CursorType.Aim);
            this.RaiseEvent<OnCodeEditorSaved, UpdateScriptEventArgs>(new UpdateScriptEventArgs(target, CodeInputField.text));
            target = null;
        }

        private void OnCancelButtonClicked()
        {
            IsActive = false;
            UIManager.SetCursor(UIManager.CursorType.Aim);
            target = null;
        }
    }
}