using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;
using TappauMVVM.Extensions;
using TappauMVVM.Properties;
// ReSharper disable UnusedMember.Global

namespace TappauMVVM.Models
{
    /// <summary>
    /// Base for models that should implement <see cref="INotifyPropertyChanged"/>.
    /// While providing a catalog of properties that have been changed since invocation.
    /// </summary>
    public class ObservableObject : PropertyChangedBase
    {
        private bool _isSelected;
        private readonly ConcurrentDictionary<string, object> _changes;

        public ObservableObject()
        {
            _changes = new ConcurrentDictionary<string, object>();
        }

        /// <summary>
        /// Is useful for when the object is being used as part of checkbox on datagrid.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetAndNotify(ref _isSelected, value);
        }

        /// <summary>
        /// Returns true, if any property has been updated on the model.
        /// </summary>
        public bool IsDirty => _changes.Any();

        private void MarkAsDirty(string propertyName, object propertyValue)
        {
            if (_changes == null)
            {
                return;
            }

            _changes[propertyName] = propertyValue;
        }

        public void MarkAllAsNotDirty()
        {
            _changes?.Clear();
        }

        public Dictionary<string, object> GetDirtyPropertyNames => new Dictionary<string, object>(_changes);

        private bool IsPropertyDirty(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException(Resources.ObservableObject_PropertyValueInvalid, nameof(propertyName));
            }

            return _changes.ContainsKey(propertyName);
        }

        public bool IsPropertyDirty<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            return IsPropertyDirty(propertyExpression.NameForProperty());
        }

        /// <summary>
        /// Override for properties that inherit from this class, that will add the change to the dirty property list with its current value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override bool SetAndNotify<T>(ref T field, T value, string propertyName = "")
        {
            MarkAsDirty(propertyName, value);
            return base.SetAndNotify(ref field, value, propertyName);
        }
    }
}
