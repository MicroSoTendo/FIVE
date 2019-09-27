using UnityEngine;
using UnityEngine.UI;
namespace FIVE.UI.AWSLEditor
{
    internal class AWSLEditorView : View<AWSLEditorView, AWSLEditorViewModel>
    {
        public GameObject AWSLEditorPanel { get; set; }
        public InputField CodeInputField { get; set; }
        public Text Text { get; set; }

        public AWSLEditorView()
        {

        }
    }
}
