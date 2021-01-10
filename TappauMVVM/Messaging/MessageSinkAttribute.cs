using System;

namespace TappauMVVM.Messaging
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
    public class MessageSinkAttribute : Attribute
    {
        /// <summary>
        /// Message Key
        /// </summary>
        public object MessageKey { get; private set; }

        /// <summary>
        /// Default constructor, sets properties to null.
        /// </summary>
        public MessageSinkAttribute()
        {
            MessageKey = null;
        }

        /// <summary>
        /// Constructor that tasks a message key
        /// </summary>
        /// <param name="messageKey">Message Key</param>
        public MessageSinkAttribute(string messageKey)
        {
            MessageKey = messageKey;
        }
    }
}