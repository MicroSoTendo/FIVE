using System;
using System.Linq.Expressions;
using static UnityEngine.Debug;

namespace FIVE.UI
{
    public class Binder<TView, TViewModel> 
        where TView : View<TView,TViewModel>, new()
        where TViewModel : ViewModel<TView, TViewModel>
    {
        public class BindingSource<TSource>
        {
            public Binding<TSource, TTarget> To<TTarget>(Expression<Func<TViewModel, TTarget>> expression, BindingMode bindingMode = BindingMode.OneWay)
            {
                //TODO: Implementation
                return default;
            }
            public Binding<TSource, EventHandler> To(Expression<Func<TViewModel, EventHandler>> expression, BindingMode bindingMode = BindingMode.OneWay)
            {
                return this.To<EventHandler>(expression, bindingMode);
            }
            public Binding<TSource, Action> To(Expression<Func<TViewModel, Action>> expression, BindingMode bindingMode = BindingMode.OneWay)
            {
                return this.To<Action>(expression, bindingMode);
            }
        }

        public BindingSource<TSource> Bind<TSource>(Expression<Func<TView, TSource>> expression)
        {
            var body = expression.Body;
            switch(body)
            {
                case MethodCallExpression methodCall:
                    Log(nameof(MethodCallExpression));
                    break;
                case UnaryExpression unary:
                    Log(nameof(UnaryExpression));
                    break;
                case MemberExpression member:
                    Log(nameof(MemberExpression));
                    break;
                default:
                    Log("default");
                    break;
            }
            return default;
        }
    }
}