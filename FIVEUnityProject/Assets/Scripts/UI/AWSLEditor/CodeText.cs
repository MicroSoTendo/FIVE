using FIVE.EventSystem;
using UnityEngine.UI;

namespace FIVE.UI.AWSLEditor
{
    public class CodeText : Text
    {
        public override string text
        {
            get => base.text;
            set
            {
                base.text = value;
                this.RaiseEvent<OnCodeTextChanged>();
            }
        }
    }
}