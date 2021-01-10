using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;
using TappauMVVM.Properties;
// ReSharper disable UnusedMember.Global

namespace TappauMVVM.Commands
{
    /// <summary>
    /// An <see cref="ICommand" /> whose delegates take any parameters for <see cref="Execute" /> and
    /// <see cref="CanExecute" />
    /// Heavily inspired by Prism.Wpf
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Command<T> : CommandBase
    {
        private readonly Action<T> _executeMethod;
        private Func<T, bool> _canExecuteMethod;

        /// <summary>
        ///     Initializes a new instance of <see cref="Command{T}" />.
        /// </summary>
        /// <param name="executeMethod">
        ///     Delegate to execute when Execute is called on the command. This can be null to just hook up
        ///     a CanExecute delegate.
        /// </param>
        /// <remarks><see cref="CanExecute(T)" /> will always return true.</remarks>
        public Command(Action<T> executeMethod) : this(o => true, executeMethod)
        {
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="Command{T}" />.
        /// </summary>
        /// <param name="executeMethod">
        ///     Delegate to execute when Execute is called on the command. This can be null to just hook up
        ///     a CanExecute delegate.
        /// </param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command. This can be null.</param>
        /// <exception cref="ArgumentNullException">
        ///     When both <paramref name="executeMethod" /> and
        ///     <paramref name="canExecuteMethod" /> are <see langword="null" />.
        /// </exception>
        public Command(Func<T, bool> canExecuteMethod, Action<T> executeMethod)
        {
            if (canExecuteMethod == null || executeMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod), Resources.CommandDelegatesCannotBeNull);
            }

            var genericTypeInfo = typeof(T).GetTypeInfo();

            // Command allows object or Nullable<>.
            // note: Nullable<> is a struct so we cannot use a class constraint.
            if (genericTypeInfo.IsValueType)
            {
                if (!genericTypeInfo.IsGenericType || !typeof(Nullable<>).GetTypeInfo()
                    .IsAssignableFrom(genericTypeInfo.GetGenericTypeDefinition().GetTypeInfo()))
                {
                    throw new InvalidCastException(Resources.CommandInvalidGenericPayloadType);
                }
            }

            _canExecuteMethod = canExecuteMethod;
            _executeMethod = executeMethod;
        }

        /// <summary>
        ///     Executes the command and invokes the <see cref="Action{T}" /> provided during construction.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        public void Execute(T parameter)
        {
            _executeMethod(parameter);
        }

        /// <summary>
        ///     Determines if the command can execute by invoking the <see cref="Func{T, Bool}" /> provided during constuction.
        /// </summary>
        /// <param name="parameter">Data used by the command to determine if it can execute.</param>
        /// <returns><see langword="True" /> if this command can be executed; otherwise, <see langword="False" /></returns>
        public bool CanExecute(T parameter)
        {
            return _canExecuteMethod(parameter);
        }

        /// <summary>
        ///     Handle the internal invocation of <see cref="ICommand.Execute(object)" />
        /// </summary>
        /// <param name="parameter">Command Parameter</param>
        protected override void Execute(object parameter)
        {
            Execute((T) parameter);
        }

        /// <summary>
        ///     Handle the internal invocation of <see cref="ICommand.CanExecute(object)" />
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns><see langword="true" /> if the Command Can Execute, otherwise <see langword="false" /></returns>
        protected override bool CanExecute(object parameter)
        {
            return CanExecute((T) parameter);
        }

        /// <summary>
        ///     Observes a property that implements INotifyPropertyChanged, and automatically calls
        ///     DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <typeparam name="TType">The type of the return value of the method that this delegate encapulates</typeparam>
        /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public Command<T> ObservesProperty<TType>(Expression<Func<TType>> propertyExpression)
        {
            ObservesPropertyInternal(propertyExpression);
            return this;
        }

        /// <summary>
        ///     Observes a property that is used to determine if this command can execute, and if it implements
        ///     INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed
        ///     notifications.
        /// </summary>
        /// <param name="canExecuteExpression">The property expression. Example: ObservesCanExecute(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public Command<T> ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
        {
            var expression =
                Expression.Lambda<Func<T, bool>>(canExecuteExpression.Body, Expression.Parameter(typeof(T), "o"));
            _canExecuteMethod = expression.Compile();
            ObservesPropertyInternal(canExecuteExpression);
            return this;
        }
    }
}