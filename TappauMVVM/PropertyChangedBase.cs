using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TappauMVVM.Extensions;

namespace TappauMVVM
{
    /// <summary>
    /// Abstract implementation of <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        /// <summary>
        ///     Raise a PropertyChanged notification from the property in the given expression, e.g. NotifyOfPropertyChange(() =>
        ///     this.Property)
        /// </summary>
        /// <typeparam name="TProperty">Type of property being notified</typeparam>
        /// <param name="property">Expression describing the property to raise a PropertyChanged notification for</param>
        protected virtual void NotifyPropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(property.NameForProperty()));
        }

        /// <summary>
        ///     Raise a PropertyChanged notification from the property with the given name
        /// </summary>
        /// <param name="propertyName">
        ///     Name of the property to raise a PropertyChanged notification for. Defaults to the calling
        ///     property
        /// </param>
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Checks if a property already matches a desired value. Sets the property and notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="field">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="onChanged">Action that is called after the property value has been changed,</param>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners. This value is optional and can be provided
        ///     automatically when invoked from compilers that support CallerMemberName.
        /// </param>
        /// <returns>
        ///     <see langword="True" /> if the value was changed, <see langword="False" /> if the existing value matched the desired
        ///     value.
        /// </returns>
        protected virtual bool SetAndNotify<T>(ref T field, T value, Action onChanged
          , [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            onChanged?.Invoke();

            NotifyPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///     Checks if a property already matches a desired value. Sets the property and notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="field">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners. This value is optional and can be provided
        ///     automatically when invoked from compilers that support CallerMemberName.
        /// </param>
        /// <returns>
        ///     <see langword="True" /> if the was changed, <see langword="False" /> if the existing value matched the desired
        ///     value.
        /// </returns>
        protected virtual bool SetAndNotify<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged

        /// <summary>
        ///     Occurs when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Raises this objects PropertyChanged event.
        /// </summary>
        /// <param name="args">The <seealso cref="PropertyChangedEventArgs" /></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        #endregion
    }
}