using System;
using System.Reflection;
using TappauMVVM.Contracts;
// ReSharper disable UnusedMember.Global

namespace TappauMVVM.Messaging
{
    public class ConcreteMessenger : IMessengerService
    {
        private readonly Messenger.InternalMessengerAccessor _internalMessengerAccessor;

        public ConcreteMessenger()
        {
            _internalMessengerAccessor = new Messenger.InternalMessengerAccessor();
        }
        
        /// <summary>
        /// This registers a Type with the mediator.  Any methods decorated with <seealso cref="MessageSinkAttribute"/> will be 
        /// registered as target method handlers for the given message key.
        /// </summary>
        /// <param name="view">Object to register</param>
        public void Register(object view)
        {
            // Look at all instance/static methods on this object type.
            foreach (var methodInfo in view.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                // See if we have a target attribute - if so, register the method as a handler.
                foreach (MessageSinkAttribute att in methodInfo.GetCustomAttributes(typeof(MessageSinkAttribute), true))
                {
                    var actionType = _internalMessengerAccessor.InitActionTOfMethodParameter(methodInfo);
                    var key = (att.MessageKey) ?? actionType;

                    _internalMessengerAccessor.RegisterHandler(key, actionType, methodInfo.IsStatic
                        ? Delegate.CreateDelegate(actionType, methodInfo)
                        : Delegate.CreateDelegate(actionType, view, methodInfo));
                }
            }
        }

        /// <summary>
        /// This method unregisters a type from the message mediator.
        /// </summary>
        /// <param name="view">Object to unregister</param>
        public void Unregister(object view)
        {
            foreach (var methodIfo in view.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                foreach (MessageSinkAttribute att in methodIfo.GetCustomAttributes(typeof(MessageSinkAttribute), true))
                {
                    var actionType = _internalMessengerAccessor.InitActionTOfMethodParameter(methodIfo);
                    var key = (att.MessageKey) ?? actionType;

                    _internalMessengerAccessor.UnregisterHandler(key, actionType, methodIfo.IsStatic
                        ? Delegate.CreateDelegate(actionType, methodIfo)
                        : Delegate.CreateDelegate(actionType, view, methodIfo));
                }
            }
        }

        /// <summary>
        /// This registers a specific method as a message handler for a specific type.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler method</param>
        public void RegisterHandler<T>(string key, Action<T> handler)
        {
            Messenger.Instance.RegisterHandler(key, handler);
        }

        /// <summary>
        /// This registers a specific method as a message handler for a specific type.
        /// </summary>
        /// <param name="handler">Handler method</param>
        public void RegisterHandler<T>(Action<T> handler)
        {
            Messenger.Instance.RegisterHandler(handler);
        }

        /// <summary>
        /// This unregisters a method as a handler.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler</param>
        public void UnregisterHandler<T>(string key, Action<T> handler)
        {
            Messenger.Instance.UnregisterHandler(key, handler);
        }

        /// <summary>
        /// This unregisters a method as a handler for a specific type
        /// </summary>
        /// <param name="handler">Handler</param>
        public void UnregisterHandler<T>(Action<T> handler)
        {
            Messenger.Instance.UnregisterHandler(handler);
        }

        /// <summary>
        /// This method broadcasts a message to all message targets for a given
        /// message key and passes a parameter.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="message">Message parameter</param>
        /// <returns>True/False if any handlers were invoked.</returns>
        public bool Publish<T>(string key, T message)
        {
            return Messenger.Instance.Publish(key, message);
        }

        /// <summary>
        /// This method broadcasts a message to all message targets for a given parameter type.
        /// If a derived type is passed, any handlers for interfaces or base types will also be
        /// invoked.
        /// </summary>
        /// <param name="message">Message parameter</param>
        /// <returns>True/False if any handlers were invoked.</returns>
        public bool Publish<T>(T message)
        {
            return Messenger.Instance.Publish(message);
        }

        /// <summary>
        /// This method broadcasts a message to all message targets for a given
        /// message key and passes a parameter.  The message targets are all called
        /// asynchronously and any resulting exceptions are ignored.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="message">Message parameter</param>
        public void PublishAsync<T>(string key, T message)
        {
            Messenger.Instance.PublishAsync(key, message);
        }

        /// <summary>
        /// This method broadcasts a message to all message targets for a given parameter type.
        /// If a derived type is passed, any handlers for interfaces or base types will also be
        /// invoked.  The message targets are all called asynchronously and any resulting exceptions
        /// are ignored.
        /// </summary>
        /// <param name="message">Message parameter</param>
        public void PublishAsync<T>(T message)
        {
            Messenger.Instance.PublishAsync(message);
        }
    }
}