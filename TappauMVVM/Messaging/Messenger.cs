using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TappauMVVM.Weak;

namespace TappauMVVM.Messaging
{
    /// <summary>
    /// This class creates a Messanger, which will loosely connect different objects together.
    /// The message handlers are organised using string-based message keys and are held in a WeakReference collection.
    /// </summary>
    public class Messenger
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Messenger instance = new Messenger();
        private readonly Dictionary<object, List<WeakAction>> _registeredHandlers =
            new Dictionary<object, List<WeakAction>>();

        private Messenger()
        {

        }

        /// <summary>
        /// Performs the actual registration of a target
        /// </summary>
        /// <param name="key">Key to store in dictionary</param>
        /// <param name="actionType">Delegate type</param>
        /// <param name="handler">Method</param>
        private void RegisterHandler(object key, Type actionType, Delegate handler)
        {
            var action = new WeakAction(handler.Target, actionType, handler.Method);

            lock (_registeredHandlers)
            {
                List<WeakAction> wr;
                if (_registeredHandlers.TryGetValue(key, out wr))
                {
                    if (wr.Count > 0)
                    {
                        var wa = wr[0];
                        if (wa.ActionType != actionType && !wa.ActionType.IsAssignableFrom(actionType))
                        {
                            throw new ArgumentException(
                                "Invalid key passed to RegisterHandler - existing handler has incompatible parameter type.");
                        }
                    }
                    
                    wr.Add(action);
                }
                else
                {
                    wr = new List<WeakAction> {action};
                    _registeredHandlers.Add(key, wr);
                }
            }
        }

        /// <summary>
        /// Performs the unregistration from a target
        /// </summary>
        /// <param name="key">Key in dictionary to remove</param>
        /// <param name="actionType">Delegate Type</param>
        /// <param name="handler">Method</param>
        private void UnregisterHandler(object key, Type actionType, Delegate handler)
        {
            lock (_registeredHandlers)
            {
                List<WeakAction> wr;
                if (_registeredHandlers.TryGetValue(key, out wr))
                {
                    wr.RemoveAll(wa => handler == wa.GetMethod() && actionType == wa.ActionType);

                    if (wr.Count == 0)
                    {
                        _registeredHandlers.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// This method broadcasts a message to all message targets for the given key and passes the parameter.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="message">Message parameter</param>
        /// <returns><see langword="True"/> if a handler was invoked, otherwise <see langword="False"/></returns>
        private bool Publish(object key, object message)
        {
            List<WeakAction> wr;
            var wrCopy = new List<WeakAction>();

            lock (_registeredHandlers)
            {
                if (!_registeredHandlers.TryGetValue(key, out wr))
                {
                    return false;
                }

                wrCopy.AddRange(wr);
            }

            foreach (var action in wrCopy.Select(cb => cb.GetMethod()))
            {
                action?.DynamicInvoke(message);
            }

            lock (_registeredHandlers)
            {
                wr.RemoveAll(wa => wa.HasBeenCollected);
            }

            return true;
        }

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static Messenger Instance
        {
            get { return Messenger.instance; }
        }

        /// <summary>
        /// This registers a Type with the mediator. Any methods decorated with <seealso cref="MessageSinkAttribute"/> will be 
        /// registered as target method handlers for the given message key.
        /// </summary>
        /// <param name="view">Object to register</param>
        public void Register(object view)
        {
            //Look at all instance/static methods on the object.
            foreach (var methodInfo in view.GetType().GetMethods(BindingFlags.Instance|BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.Public))
            {
                //See if any methods are tagged with the attribute and register as a handler
                foreach (MessageSinkAttribute att in methodInfo.GetCustomAttributes(typeof(MessageSinkAttribute), true))
                {
                    var actionType = InitActionTOfMethodParameter(methodInfo);
                    var key = (att.MessageKey) ?? actionType;
                    RegisterHandler(key, actionType, methodInfo.IsStatic
                        ? Delegate.CreateDelegate(actionType, methodInfo)
                        : Delegate.CreateDelegate(actionType, view, methodInfo.Name));
                }
            }
        }

        private Type InitActionTOfMethodParameter(MethodInfo methodInfo)
        {
            var methodParams = methodInfo.GetParameters();
            if (methodParams.Length != 1)
            {
                throw new InvalidCastException($"Cannot cast {methodInfo.Name} to Action<T> delegate type.");
            }

            var actionType = typeof(Action<>).MakeGenericType(methodParams[0].ParameterType);
            return actionType;
        }

        /// <summary>
        /// Unregisters a type from the Messenger instance.
        /// </summary>
        /// <param name="view">Object to unregister.</param>
        public void Unregister(object view)
        {
            foreach (var methodInfo in view.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static |
                                                                 BindingFlags.NonPublic | BindingFlags.Public))
            {
                foreach (MessageSinkAttribute att in methodInfo.GetCustomAttributes(typeof(MessageSinkAttribute), true))
                {
                    var actionType = InitActionTOfMethodParameter(methodInfo);
                    var key = (att.MessageKey) ?? actionType;

                    UnregisterHandler(key, actionType, methodInfo.IsStatic
                        ? Delegate.CreateDelegate(actionType, methodInfo)
                        : Delegate.CreateDelegate(actionType, view, methodInfo.Name));
                }
            }
        }

        /// <summary>
        /// Registers a specific method as a message handler for a specific type.
        /// </summary>
        /// <param name="key">Message Key</param>
        /// <param name="handler">Handler method.</param>
        public void RegisterHandler<T>(string key, Action<T> handler)
        {
            RegisterHandler(key, handler.GetType(), handler);
        }

        /// <summary>
        /// Registers a spefic method as a message handler for a specific type.
        /// </summary>
        /// <param name="handler">Handler method.</param>
        public void RegisterHandler<T>(Action<T> handler)
        {
            RegisterHandler(typeof(Action<T>), handler.GetType(), handler);
        }

        /// <summary>
        /// Unregisters a method as a handler.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler method</param>
        public void UnregisterHandler<T>(string key, Action<T> handler)
        {
            UnregisterHandler(key, handler.GetType(), handler);
        }

        /// <summary>
        /// Unregister a method as a handler for a specific type.
        /// </summary>
        /// <param name="handler">Handler Method</param>
        public void UnregisterHandler<T>(Action<T> handler)
        {
            UnregisterHandler(typeof(Action<T>), handler.GetType(), handler);
        }

        /// <summary>
        /// Broadcast a message to all message targets for a given <paramref name="key"/> and passes a parameter.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="message">Message parameter</param>
        /// <returns><see langword="True"/> if a handler was invoked, otherwise <see langword="False"/></returns>
        public bool Publish<T>(string key, T message)
        {
            return Publish((object) key, message);
        }

        /// <summary>
        /// Broadcast a message to all message targets for a given parameter type.
        /// If a derived type is passed, any handlers for interfaces or base types will also be invoked.
        /// </summary>
        /// <param name="message">Message key</param>
        /// <returns><see langword="True"/> if a handler was invoked, otherwise <see langword="False"/></returns>
        public bool Publish<T>(T message)
        {
            var actionType = typeof(Action<>).MakeGenericType(typeof(T));
            if (_registeredHandlers == null)
            {
                return false;
            }

            var keyList =
                _registeredHandlers.Keys.Where(key => key is Type type && type.IsAssignableFrom(actionType));

            return keyList.Aggregate(false, (current, key) => current | Publish(key, message));
        }

        /// <summary>
        /// Broadcasts a message to all message targets for a given <paramref name="key"></paramref> and passes the <paramref name="message"></paramref>.
        /// The message targets are all called asyncronusly and any resulting exceptions are ignored.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="message">Message parameter</param>
        public void PublishAsync<T>(string key, T message)
        {
            Func<string, T, bool> func = Publish;
            func.BeginInvoke(key, message, res =>
            {
                try
                {
                    func.EndInvoke(res);
                }
                catch
                {
                    // ignored
                }
            }, null);
        }

        /// <summary>
        /// Broadcasts a message to all message targets for a given <paramref name="message"/>
        /// The message targets are all called asyncronusly and any resulting exceptions are ignored.
        /// </summary>
        /// <param name="message">Message parameter</param>
        public void PublishAsync<T>(T message)
        {
            Func<T, bool> func = Publish;
            func.BeginInvoke(message, res =>
            {
                try
                {
                    func.EndInvoke(res);
                }
                catch
                {
                    // ignored
                }
            }, null);
        }
        
        public class InternalMessengerAccessor
        {
            public void RegisterHandler(object key, Type actionType, Delegate handler)
            {
                instance.RegisterHandler(key, actionType, handler);
            }

            public void UnregisterHandler(object key, Type actionType, Delegate handler)
            {
                instance.UnregisterHandler(key, actionType, handler);
            }

            public Type InitActionTOfMethodParameter(MethodInfo methodInfo)
            {
                return instance.InitActionTOfMethodParameter(methodInfo);
            }
        }
    }

    
}