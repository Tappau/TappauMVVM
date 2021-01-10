using System;

namespace TappauMVVM.Contracts
{
    /// <summary>
    ///     Interface that defines if th eobject instance is active and notifies when the activity changes.
    /// </summary>
    public interface IActiveAware
    {
        /// <summary>
        ///     Gets or Sets a value indicating whether the object is active.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        ///     Notifies that the value for <see cref="IsActive" /> property has changed.
        /// </summary>
        event EventHandler IsActiveChanged;
    }
}