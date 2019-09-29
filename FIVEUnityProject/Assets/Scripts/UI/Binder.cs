using FIVE.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI
{
    public class Binder<TView, TViewModel>
        where TView : View<TView, TViewModel>, new()
        where TViewModel : ViewModel<TView, TViewModel>
    {

        private readonly List<IBindingSource> bindingSources = new List<IBindingSource>();
        private View<TView, TViewModel> view;
        private ViewModel<TView, TViewModel> viewModel;

        public Binder(View<TView, TViewModel> view, ViewModel<TView, TViewModel> viewModel)
        {
            this.view = view;
            this.viewModel = viewModel;
        }
        public interface IBindingSource
        {
            void ReBind(View view, ViewModel viewModel);
        }
        public class BindingSource<TSource> : IBindingSource
            where TSource : class
        {
            private Expression<Func<TView, TSource>> expression;
            private MonoBehaviour uiElement;
            private object bindingMember;
            private Action bindingAction;
            private ViewModel<TView, TViewModel> viewModel;

            public BindingSource(
                Expression<Func<TView, TSource>> expression,
                View<TView, TViewModel> view,
                ViewModel<TView, TViewModel> viewModel)
            {
                SetSourceInfo(expression, view, viewModel);
            }

            private void SetSourceInfo(
                Expression<Func<TView, TSource>> expression,
                View<TView, TViewModel> view,
                ViewModel<TView, TViewModel> viewModel)
            {
                this.expression = expression;
                if (expression.Body is MemberExpression memberExpression)
                {
                    MemberExpression uiElementExp = memberExpression.Expression as MemberExpression;
                    PropertyInfo uiElementProperty = uiElementExp.Member as PropertyInfo;
                    uiElement = uiElementProperty.GetValue(view) as MonoBehaviour;
                    PropertyInfo bindingMemberProperty = memberExpression.Member as PropertyInfo;
                    bindingMember = bindingMemberProperty.GetValue(uiElement);
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
                bindingAction = () =>
                {
                    if (bindingMember is Button.ButtonClickedEvent clickEvent)
                    {
                        Func<TViewModel, EventHandler> compiledFunc = expression.Compile();
                        clickEvent.AddListener(delegate
                        {
                            compiledFunc(viewModel as TViewModel)(uiElement, EventArgs.Empty);
                        });
                    }
                };
                MainThreadDispatcher.Schedule(bindingAction);
            }

            public void ToBroadcast<T>()
            {
                //TODO: Implement other objects event
                if (bindingMember is Button.ButtonClickedEvent clickEvent)
                {
                    clickEvent.AddListener(async () => { await uiElement.gameObject.RaiseEventAsync<T>(EventArgs.Empty); });
                }
            }

            public void ReBind(View v, ViewModel vm)
            {
                SetSourceInfo(expression, (View<TView, TViewModel>)v, (ViewModel<TView, TViewModel>)vm);
                bindingAction();
            }
        }

        public BindingSource<TSource> Bind<TSource>(Expression<Func<TView, TSource>> expression) where TSource : class
        {
            BindingSource<TSource> bindingSource = new BindingSource<TSource>(expression, view, viewModel);
            bindingSources.Add(bindingSource);
            return bindingSource;
        }

        public void ReBind(View<TView, TViewModel> newView, ViewModel<TView, TViewModel> newViewModel)
        {
            view = newView;
            viewModel = newViewModel;
            foreach (IBindingSource bindingSource in bindingSources)
            {
                bindingSource.ReBind(newView, newViewModel);
            }
        }
    }
}