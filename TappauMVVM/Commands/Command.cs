using System;
using System.Linq.Expressions;
using System.Windows.Input;
using TappauMVVM.Properties;
// ReSharper disable UnusedMember.Global

namespace TappauMVVM.Commands
{
    /// <summary>
    ///     An <see cref="ICommand" /> whose delegates do not take any parameters for <see cref="Execute()" /> and
    ///     <see cref="CanExecute()" />
    /// </summary>
    /// <see cref="CommandBase" />
    /// <see cref="Command{T}" />
    public class Command : CommandBase
    {
        private Func<bool> _canExecuteMethod;
        private readonly Action _executeMethod;

        /// <summary>
        ///     Creates a new instance of <see cref="Command" /> with the <see cref="Action" /> to invoke on execution.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action" /> to invoke when <see cref="ICommand.Execute(object)" /> is called.</param>
        public Command(Action executeMethod) : this(() => true, executeMethod)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Command" /> with the <see cref="Action" /> to invoke on execution
        ///     and a <see langword="Func" /> to query for determining if the command can execute.
        /// </summary>
        /// <param name="canExecuteMethod">
        ///     The <see cref="Func{TResult}" /> to invoke when <see cref="ICommand.CanExecute" /> is
        ///     called
        /// </param>
        /// <param name="executeMethod">The <see cref="Action" /> to invoke when <see cref="ICommand.Execute" /> is called.</param>
        /// <exception cref="ArgumentNullException">
        ///     When both <paramref name="executeMethod" /> and
        ///     <paramref name="canExecuteMethod" /> are <see langword="null" />.
        /// </exception>
        public Command(Func<bool> canExecuteMethod, Action executeMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException(nameof(canExecuteMethod), Resources.CommandDelegatesCannotBeNull);
            }

            _canExecuteMethod = canExecuteMethod;
            _executeMethod = executeMethod;
        }

        /// <summary>
        ///     Handle the internal invocation of <see cref="ICommand.Execute(object)" />
        /// </summary>
        /// <param name="parameter">Command Parameter</param>
        protected override void Execute(object parameter)
        {
            Execute();
        }

        /// <summary>
        ///     Execute the command.
        /// </summary>
        public void Execute()
        {
            _executeMethod();
        }

        /// <summary>
        ///     Handle the internal invocation of <see cref="ICommand.CanExecute(object)" />
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns><see langword="true" /> if the Command Can Execute, otherwise <see langword="false" /></returns>
        protected override bool CanExecute(object parameter)
        {
            return CanExecute();
        }


        /// <summary>
        ///     Determines if the command can be executed.
        /// </summary>
        /// <returns>
        ///     Returns <see langword="True" /> if the command can execute, otherwise returns <see langword="False" />
        /// </returns>
        private bool CanExecute()
        {
            return _canExecuteMethod();
        }

        /// <summary>
        ///     Observes a property that implements INotifyPropertyChanged, and automatically calls
        ///     DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
        /// <returns>The current instance of Command</returns>
        public Command ObservesProperty<T>(Expression<Func<T>> propertyExpression)
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
        public Command ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
        {
            _canExecuteMethod = canExecuteExpression.Compile();
            ObservesPropertyInternal(canExecuteExpression);
            return this;
        }
    }
}