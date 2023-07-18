// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

#if MRTK_SPATIAL_PRESENT
using Microsoft.MixedReality.Toolkit.SpatialManipulation;
#endif

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// The semantic action associated with a button on a dialog.
    /// </summary>
    /// <remarks>
    /// Custom dialogs may specify additional button types. In such a case, the <see cref="DialogButtonType"/> value
    /// should be <see cref="DialogButtonType.Other"/> and the event receiver should cast the event arguments 
    /// to a custom type so to obtain more specific information about the action.
    /// </remarks>d
    public enum DialogButtonType
    {
        /// <summary>
        /// An affirmative button, such as an "okay" or "yes" button, used to confirm an action or inquiry.
        /// </summary>
        Positive = 0,

        /// <summary>
        /// An negative button, such as a "no" button, used to reject an action or inquiry.
        /// </summary>
        Negative = 1,

        /// <summary>
        /// An neutral button, such as a "cancel" button, used to neither confirm or reject an action or inquiry.
        /// </summary>
        Neutral = 2,

        /// <summary>
        /// Represents extended button behavior.
        /// </summary>
        /// <remarks>
        /// When receiving this value, the receiver should cast the associated event arguments 
        /// to a custom type so to obtain more specific information about the button.
        /// </remarks>
        Other = 255
    }

    /// <summary>
    /// An IDialog hydrates and controls the various sub-components
    /// of the dialog view.
    /// </summary>
    /// <remarks>
    /// IDialogs are typically spawned, pooled, and killed
    /// by <see cref="Microsoft.MixedReality.Toolkit.UX.DialogPool">DialogPools</see>. 
    /// Generally, developers should not directly manage or instantiate instances of their dialogs,
    /// as it is essential that they are pooled and managed correctly by a coordinator.
    /// </remarks>
    public interface IDialog
    {
        /// <summary>
        /// Sets the header text on the dialog. This is generally displayed
        /// in a more prominent position and/or with more prominent styling
        /// than the body text.
        /// </summary>
        /// <returns>This <see cref="IDialog"/> object, so to enable function chaining.</returns>
        IDialog SetHeader(string header);

        /// <summary>
        /// Sets the body text on the dialog.
        /// </summary>
        /// <returns>This <see cref="IDialog"/> object, so to enable function chaining.</returns>
        IDialog SetBody(string body);
        
        /// <summary>
        /// Adds a positive option on the Dialog. <paramref name="action"/> will be invoked
        /// when the option is selected. <paramref name="label"/> will be affixed to the
        /// associated button.
        /// </summary>
        /// <param name="label"> The text to affix to the button.</param>
        /// <param name="action"> The action to invoke when the button is selected.</param>
        /// <returns>This <see cref="IDialog"/> object, so to enable function chaining.</returns>
        IDialog SetPositive(string label, Action<DialogButtonEventArgs> action = null);

        /// <summary>
        /// Adds a negative option on the Dialog. <paramref name="action"/> will be invoked
        /// when the option is selected. <paramref name="label"/> will be affixed to the
        /// associated button.
        /// </summary>
        /// <param name="label">The text to affix to the button.</param>
        /// <param name="action">The action to invoke when the button is selected.</param>
        /// <returns>This <see cref="IDialog"/> object, so to enable function chaining.</returns>
        IDialog SetNegative(string label, Action<DialogButtonEventArgs> action = null);

        /// <summary>
        /// Adds a neutral option on the Dialog. <paramref name="action"/> will be invoked
        /// when the option is selected. <paramref name="label"/> will be affixed to the
        /// associated button.
        /// </summary>
        /// <param name="label"> The text to affix to the button.</param>
        /// <param name="action"> The action to invoke when the button is selected.</param>
        /// <returns>This <see cref="IDialog"/> object, so to enable function chaining.</returns>
        IDialog SetNeutral(string label, Action<DialogButtonEventArgs> action = null);
        
        /// <summary>
        /// This event is fired when the dialog is dismissed, either
        /// through user inaction, or by some other foreground action
        /// taking precedence.
        /// </summary>
        /// <remarks>
        /// For other actions, such as button presses and text field submits,
        /// provide your own delegates through the fluent methods. For example,
        /// use <see cref="SetPositive"/> for these types of actions.
        ///
        /// Any asynchronous method awaiting on the result of <see cref="ShowAsync"/> will
        /// be unblocked when this event is fired.
        /// </remarks>
        Action<DialogDismissedEventArgs> OnDismissed { get; set; }

        /// <summary>
        /// Clears all content, events, and configuration from the dialog.
        /// Useful when pooling Dialog objects, to ensure that subsequent
        /// uses of the object don't retain stale data.
        /// </summary>
        /// <remarks>
        /// When implementing custom dialog types, be sure to override
        /// this method to clear any custom state or fields.
        /// </remarks>
        void Reset();
        
        /// <summary>
        /// Shows this <see cref="IDialog"/> object. Call this method after the content and actions
        /// have been specified with the fluent methods.
        /// </summary>
        /// <remarks>
        /// Call this method after the content and actions have been specified with the fluent methods.
        /// These methods include <see cref="SetHeader"/>, <see cref="SetBody"/>, and  <see cref="SetPositive"/>.
        ///
        /// When implementing custom dialog types, be sure to override this to
        /// show your custom dialog in an appropriate way.
        /// </remarks>
        /// <returns>This <see cref="IDialog"/> object, so to enable function chaining.</returns>
        IDialog Show();

        /// <summary>
        /// An asynchronous operation that returns a <see cref="DialogDismissedEventArgs"/> that represents the result
        /// of the user's choice. The task will complete once the dialog dismissed, either by user action or by some 
        /// other foreground action. If the user does not select an option, the 
        /// <see cref="DialogDismissedEventArgs.Choice"/> property will be null.
        /// </summary>
        /// <remarks>
        /// This is usually implemented by a TaskCompletionSource listening on the
        /// the <see cref="OnDismissed"/> delegate. This allows for either synchronous
        /// or asynchronous usage of the <see cref="IDialog"/>.
        /// </remarks>
        Task<DialogDismissedEventArgs> ShowAsync();

        /// <summary>
        /// Dismisses the dialog. Unsubscribes all listeners from the dialog's
        /// events, plays the dismiss animation, and then invokes <see cref="OnDismissed"/> immediately.
        /// </summary>
        /// <remarks>
        /// When creating a <see cref="IDialog"/>, the implementation should unsubscribe listeners 
        /// from their custom events, if any. Also, this base implementation should be called
        /// after the derived implementation, as the provided base implementation will trigger 
        /// the dismissal animation and invoke the <see cref="OnDismissed"/> event for the application.
        ///
        /// Any asynchronous methods awaiting on the result of <see cref="ShowAsync"/> will be completed at the
        /// same time as the OnDismissed delegate is invoked.
        /// </remarks>
        void Dismiss();

        /// <summary>
        /// The dialog's root Unity game object, used for setting visibility.
        /// </summary>
        GameObject VisibleRoot { get; }
    }
}