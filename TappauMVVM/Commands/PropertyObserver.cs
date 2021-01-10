using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using TappauMVVM.Properties;

namespace TappauMVVM.Commands
{
    /// <summary>
    ///     Provides a way to observe property changes of INotifyPropertyChanged objects and invokes a custom action when the
    ///     PropertyChanged event is fired.
    /// </summary>
    internal class PropertyObserver
    {
        private readonly Action _action;

        private PropertyObserver(Expression propExpression, Action action)
        {
            _action = action;
            SubscribeListeners(propExpression);
        }

        private void SubscribeListeners(Expression propExpression)
        {
            var propNameStack = new Stack<PropertyInfo>();
            while (propExpression is MemberExpression temp) // Gets the root of the property chain.
            {
                propExpression = temp.Expression;
                propNameStack.Push(temp.Member as PropertyInfo); // Records the member info as property info
            }

            if (!(propExpression is ConstantExpression constantExpression))
            {
                throw new NotSupportedException(Resources.PropertyObserverOperationNotSupported);
            }

            var propObserverNodeRoot = new PropertyObserverNode(propNameStack.Pop(), _action);
            var previousNode = propObserverNodeRoot;
            foreach (var propName in propNameStack) // Create a node chain that corresponds to the property chain.
            {
                var currentNode = new PropertyObserverNode(propName, _action);
                previousNode.Next = currentNode;
                previousNode = currentNode;
            }

            var propOwnerObject = constantExpression.Value;

            if (!(propOwnerObject is INotifyPropertyChanged inpcObject))
            {
                throw new InvalidOperationException(
                    string.Format(Resources.PropertyObserver_PropertyDoesNotImplementINotifyPropertyChanged
                      , propObserverNodeRoot.PropertyInfo.Name));
            }

            propObserverNodeRoot.SubscribeListenerFor(inpcObject);
        }

        internal static PropertyObserver Observes<T>(Expression<Func<T>> propExpression, Action action)
        {
            return new PropertyObserver(propExpression.Body, action);
        }
    }
}