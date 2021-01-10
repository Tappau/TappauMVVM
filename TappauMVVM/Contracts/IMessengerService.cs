using System;
using TappauMVVM.Messaging;

namespace TappauMVVM.Contracts
{
    public interface IMessengerService
    {
        /// <summary>
        ///     This registers a Type with the mediator. Any methods decorated with <seealso cref="MessageSinkAttribute" /> will be
        ///     registered as target method handlers for the given message key.
        /// </summary>
        /// <param name="view">Object to register</param>
        void Register(object view);

        /// <summary>
        ///     Unregisters a type from the Messenger instance.
        /// </summary>
        /// <param name="view">Object to unregister.</param>
        void Unregister(object view);

        /// <summary>
        ///     Registers a specific method as a message handler for a specific type.
        /// </summary>
        /// <param name="key">Message Key</param>
        /// <param name="handler">Handler method.</param>
        void RegisterHandler<T>(string key, Action<T> handler);

        /// <summary>
        ///     Registers a spefic method as a message handler for a specific type.
        /// </summary>
        /// <param name="handler">Handler method.</param>
        void RegisterHandler<T>(Action<T> handler);

        /// <summary>
        ///     Unregisters a method as a handler.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler method</param>
        void UnregisterHandler<T>(string key, Action<T> handler);

        /// <summary>
        ///     Unregister a method as a handler for a specific type.
        /// </summary>
        /// <param name="handler">Handler Method</param>
        void UnregisterHandler<T>(Action<T> handler);

        /// <summary>
        ///     Broadcast a message to all message targets for a given <paramref name="key"></paramref> and passes a parameter.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="message">Message parameter</param>
        /// <returns><see langword="True" /> if a handler was invoked, otherwise <see langword="False" /></returns>
        bool Publish<T>(string key, T message);

        /// <summary>
        ///     Broadcast a message to all message targets for a given parameter type.
        ///     If a derived type is passed, any handlers for interfaces or base types will also be invoked.
        /// </summary>
        /// <param name="message">Message key</param>
        /// <returns><see langword="True" /> if a handler was invoked, otherwise <see langword="False" /></returns>
        bool Publish<T>(T message);

        /// <summary>
        ///     Broadcasts a message to all message targets for a given <paramref name="key"></paramref> and passes the
        ///     <paramref name="message"></paramref>.
        ///     The message targets are all called asyncronusly and any resulting exceptions are ignored.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="message">Message parameter</param>
        void PublishAsync<T>(string key, T message);

        /// <summary>
        ///     Broadcasts a message to all message targets for a given <paramref name="message" />
        ///     The message targets are all called asyncronusly and any resulting exceptions are ignored.
        /// </summary>
        /// <param name="message">Message parameter</param>
        void PublishAsync<T>(T message);
    }
}