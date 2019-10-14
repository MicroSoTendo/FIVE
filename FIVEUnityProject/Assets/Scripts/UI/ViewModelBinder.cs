using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FIVE.UI
{
    public abstract partial class ViewModel
    {
        protected ButtonSource Bind(Button button)
        {
            return new ButtonSource(button);
        }

        protected UnBindButtonSource UnBind(Button button)
        {
            return new UnBindButtonSource(button);
        }

        public class ViewModelComparer : IComparer<ViewModel>
        {
            public int Compare(ViewModel x, ViewModel y)
            {
                if (x == null)
                {
                    return y != null ? -1 : 0;
                }
                return y != null ? x.ZIndex.CompareTo(y.ZIndex) : 1;
            }
        }

        protected abstract class BindingSource<T>
        {
            protected T Source { get; }
            protected BindingSource(T source)
            {
                Source = source;
            }
        }

        protected class ButtonSource : BindingSource<Button>
        {
            public ButtonSource(Button button) : base(button) { }

            public void To(UnityAction OnButtonClicked)
            {
                Source.onClick.AddListener(OnButtonClicked);
            }
        }

        protected class UnBindButtonSource : BindingSource<Button>
        {
            public UnBindButtonSource(Button source) : base(source) { }

            public void With(UnityAction OnButtonClicked)
            {
                Source.onClick.RemoveListener(OnButtonClicked);
            }

            public void All()
            {
                Source.onClick.RemoveAllListeners();
            }
        }

        protected class TextSource : BindingSource<Text>
        {
            public TextSource(Text source) : base(source) { }
            public void To(ref string s) { }
        }
    }
}