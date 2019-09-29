using UnityEngine;
using UnityEngine.UI;
namespace FIVE.UI.AWSLEditor
{
    internal class AWSLEditorView : View<AWSLEditorView, AWSLEditorViewModel>
    {
        [UIElement]
        public GameObject AWSLEditorPanel { get; set; }
        [UIElement]
        public InputField CodeInputField { get; set; }
        [UIElement(nameof(CodeInputField), TargetType.Property)]
        public CodeText CodeText { get; set; }
        [UIElement]
        public Button SaveButton { get; set; }
        [UIElement]
        public Button CancelButton { get; set; }
        [UIElement]
        public Text LineNumber { get; set; }
    }
}
