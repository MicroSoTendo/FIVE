using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Debug;

namespace FIVE.UI
{
    public class Binder<TView, TViewModel>
        where TView : View<TView, TViewModel>, new()
        where TViewModel : ViewModel<TView, TViewModel>
    {

        private View<TView, TViewModel> view;
        private ViewModel<TView, TViewModel> viewModel;

        public Binder(View<TView, TViewModel> view, ViewModel<TView, TViewModel> viewModel)
        {
            this.view = view;
            this.viewModel = viewModel;
        }
        private List<Binding> bindings;

        public class BindingSource<TSource>
        {
            public BindingSource(Expression<Func<TView, TSource>> expression)
            {
                Helper(expression);

            }
            public void To<TTarget>(Expression<Func<TViewModel, TTarget>> expression,
                BindingMode bindingMode = BindingMode.OneWay)
            {
                //TODO: Implementation
            }
            public void To(Expression<Action<TViewModel>> expression,
                BindingMode bindingMode = BindingMode.OneWay)
            {
                //TODO: Implementation
            }
            //public void To(Expression<Func<TViewModel, TTarget>> expression,
            //    BindingMode bindingMode = BindingMode.OneWay)
            //{
            //    //TODO: Implementation
            //}

            //public void To(Expression<Func<TViewModel, EventHandler>> expression,
            //    BindingMode bindingMode = BindingMode.OneWay)
            //{
            //    this.To<EventHandler>(expression, bindingMode);
            //}
            //public void To(Expression<Func<TViewModel, Action>> expression,
            //    BindingMode bindingMode = BindingMode.OneWay)
            //{
            //    this.To<Action>(expression, bindingMode);
            //}

            private void Helper(Expression<Func<TView, TSource>> expression)
            {
                switch (expression.Body)
                {
                    case LambdaExpression lambda:
                        break;
                    case MethodCallExpression methodCall:
                        break;
                    case MemberExpression member:
                        var rootType = member.Member.DeclaringType;
                        if(rootType.IsAssignableFrom(typeof(Button)))
                        {
                            Debug.Log(rootType);
                            var name = member.Member.Name;
                            Debug.Log(name);
                        }
                        // (member.Member as PropertyInfo).GetValue()
                        break;
                    case UnaryExpression unary:
                        break;
                    default:
                        break;
                }
            }
        }

        private class Binding
        {
            //Expression<Func<TView, TSource>>[] sourceExpressions;
        }
        
        public BindingSource<TSource> Bind<TSource>(Expression<Func<TView, TSource>> expression)
        {
            return new BindingSource<TSource>(expression);
        }
    }
}