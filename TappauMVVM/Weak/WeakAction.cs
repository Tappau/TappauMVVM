using System;
using System.Diagnostics;
using System.Reflection;

namespace TappauMVVM.Weak
{
    /// <summary>
    /// This class creates a weak delegate of form <see cref="Action{T}"/>
    /// </summary>
    public class WeakAction
    {
        #region Internal

        private readonly WeakReference _target;
        private readonly Type _ownerType;
        private readonly Type _actionType;
        private readonly string _methodName;

        #endregion

        #region Public Properties/Methods

        public WeakAction(object target, Type actionType, MethodBase methodBase)
        {
            if (target == null)
            {
                Debug.Assert(methodBase.IsStatic);
                _ownerType = methodBase.DeclaringType;
            }
            else
            {
                _target = new WeakReference(target);
                _methodName = methodBase.Name;
                _actionType = actionType;
            }
        }

        public Type ActionType => _actionType;

        public bool HasBeenCollected => _ownerType == null && (_target == null || !_target.IsAlive);

        public Delegate GetMethod()
        {
            if (_ownerType != null)
            {
                return Delegate.CreateDelegate(_actionType, _ownerType, _methodName);
            }

            if (_target != null && _target.IsAlive)
            {
                var target = _target.Target;
                if (target != null)
                {
                    return Delegate.CreateDelegate(_actionType, target, _methodName);
                }
            }

            return null;
        }

        #endregion
    }
}