using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FIVE.EventSystem;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI
{
    public class Binder<TView, TViewModel>
        where TView : View<TView, TViewModel>, new()
        where TViewModel : ViewModel<TView, TViewModel>
    {

        private readonly View<TView, TViewModel> view;
        private readonly ViewModel<TView, TViewModel> viewModel;

        public Binder(View<TView, TViewModel> view, ViewModel<TView, TViewModel> viewModel)
        {
            this.view = view;
            this.viewModel = viewModel;
        }
        private List<Binding> bindings;

        public class BindingSource<TSource> where TSource : class
        {
            public Expression<Func<TView, TSource>> Expression { get; }

            public MonoBehaviour UIElement;
            public object BindingMember;

            private ViewModel<TView, TViewModel> viewModel;
            public BindingSource(Expression<Func<TView, TSource>> expression, 
                View<TView, TViewModel> view,
                ViewModel<TView, TViewModel> viewModel)
            {
                Expression = expression;
                if (expression.Body is MemberExpression memberExpression)
                {
                    var UIElementExp = memberExpression.Expression as MemberExpression;
                    var UIElementProperty = UIElementExp.Member as PropertyInfo;
                    UIElement = UIElementProperty.GetValue(view) as MonoBehaviour;
                    var BindingMemberProperty = memberExpression.Member as PropertyInfo;
                    BindingMember = BindingMemberProperty.GetValue(UIElement);
                }
                this.viewModel = viewModel;
            }
            public void To<TTarget>(Expression<Func<TViewModel, TTarget>> expression,
                BindingMode bindingMode = BindingMode.OneWay)
            {
                //TODO: Implementation
            }
            public void To(Expression<Func<TViewModel, EventHandler>> expression,
                BindingMode bindingMode = BindingMode.OneWay)
            {
                if (BindingMember is Button.ButtonClickedEvent clickEvent)
                {
                    var compiledFunc = expression.Compile();
                    clickEvent.AddListener(delegate { compiledFunc(viewModel as TViewModel)(UIElement, EventArgs.Empty); });
                }
            }

            public void ToBroadcast<T>()
            {
                //TODO: Implement other objects event
                if (BindingMember is Button.ButtonClickedEvent clickEvent)
                {
                    clickEvent.AddListener(async () => { await UIElement.gameObject.RaiseEventAsync<T>(EventArgs.Empty); });
                }
            }
        }

        private class Binding
        {
            //Expression<Func<TView, TSource>>[] sourceExpressions;
        }
        
        public BindingSource<TSource> Bind<TSource>(Expression<Func<TView, TSource>> expression) where TSource : class
        {
            return new BindingSource<TSource>(expression, view, viewModel);
        }
    }
}