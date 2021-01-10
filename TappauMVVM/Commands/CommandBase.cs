using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using System.Windows.Input;
using TappauMVVM.Contracts;
using TappauMVVM.Properties;

namespace TappauMVVM.Commands
{
    ///Heavily inspired by Prism.Wpf
    /// <summary>
    ///     An <see cref="ICommand" /> whose delegates can be attached for <see cref="CanExecute" /> and <see cref="Execute" />
    /// </summary>
    public abstract class CommandBase : ICommand, IActiveAware, IRaiseCanExecuteChanged
    {
        private readonly HashSet<string> _observedPropertiesExpressions = new HashSet<string>();
        private readonly SynchronizationContext _synchronizationContext;
        private bool _isActive;

        protected CommandBase()
        {
            _synchronizationContext = SynchronizationContext.Current;
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value)
                {
                    return;
                }

                _isActive = value;
                OnIsActiveChanged();
            }
        }

        /// <summary>
        ///     Notifies that the value for <see cref="IActiveAware.IsActive" /> property has changed.
        /// </summary>
        public event EventHandler IsActiveChanged;


        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }


        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///     Raises <see cref="CanExecuteChanged" /> so every command invoker can requery to check if the command can execute.
        ///     <remarks>Note thiat this will trigger the execution of <see cref="CanExecuteChanged" /> once for each invoker.</remarks>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public virtual void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler == null)
            {
                return;
            }

            if (_synchronizationContext != null && _synchronizationContext != SynchronizationContext.Current)
            {
                _synchronizationContext.Post(o => handler.Invoke(this, EventArgs.Empty), null);
            }
            else
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Handle the internal invocation of <see cref="ICommand.Execute(object)" />
        /// </summary>
        /// <param name="parameter">Command Parameter</param>
        protected abstract void Execute(object parameter);

        /// <summary>
        ///     Handle the internal invocation of <see cref="ICommand.CanExecute(object)" />
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns><see langword="true" /> if the Command Can Execute, otherwise <see langword="false" /></returns>
        protected abstract bool CanExecute(object parameter);

        protected void ObservesPropertyInternal<T>(Expression<Func<T>> propertyExpression)
        {
            if (_observedPropertiesExpressions.Contains(propertyExpression.ToString()))
            {
                throw new ArgumentException(
                    string.Format(Resources.PropertyExpressionAlreadyObserved, propertyExpression)
                  , nameof(propertyExpression));
            }

            _observedPropertiesExpressions.Add(propertyExpression.ToString());
            PropertyObserver.Observes(propertyExpression, RaiseCanExecuteChanged);
        }

        /// <summary>
        ///     This raises the <see cref="IsActiveChanged" /> event.
        /// </summary>
        protected virtual void OnIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}