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
    /// Derived/custom Dialogs may return Other types of buttons.
    /// </summary>
    public enum DialogButtonType
    {
        Positive = 0,
        Negative = 1,
        Neutral = 2,
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
    /// as it is essential that they are pooled and managed correctly by a pooler.
    /// </remarks>
    public interface IDialog
    {
        /// <summary>
        /// Sets the header text on the dialog. This is generally displayed
        /// in a more prominent position and/or with more prominent styling
        /// than the body text.
        /// </summary>
        /// <returns> Itself, for chaining. </returns>
        IDialog SetHeader(string header);

        /// <summary>
        /// Sets the body text on the dialog.
        /// </summary>
        /// <returns> Itself, for chaining. </returns>
        IDialog SetBody(string body);
        
        /// <summary>
        /// Adds a positive option on the Dialog. <paramref name="action"/> will be invoked
        /// when the option is selected. <paramref name="label"/> will be affixed to the
        /// associated button.
        /// </summary>
        /// <param name="label"/> The text to affix to the button.</param>
        /// <param name="action"/> The action to invoke when the button is selected.</param>
        /// <returns> Itself, for chaining. </returns>
        IDialog SetPositive(string label, Action<DialogButtonEventArgs> action = null);

        /// <summary>
        /// Adds a negative option on the Dialog. <paramref name="action"/> will be invoked
        /// when the option is selected. <paramref name="label"/> will be affixed to the
        /// associated button.
        /// </summary>
        /// <param name="label"/> The text to affix to the button.</param>
        /// <param name="action"/> The action to invoke when the button is selected.</param>
        /// <returns> Itself, for chaining. </returns>
        IDialog SetNegative(string label, Action<DialogButtonEventArgs> action = null);

        /// <summary>
        /// Adds a neutral option on the Dialog. <paramref name="action"/> will be invoked
        /// when the option is selected. <paramref name="label"/> will be affixed to the
        /// associated button.
        /// </summary>
        /// <param name="label"/> The text to affix to the button.</param>
        /// <param name="action"/> The action to invoke when the button is selected.</param>
        /// <returns> Itself, for chaining. </returns>
        IDialog SetNeutral(string label, Action<DialogButtonEventArgs> action = null);
        
        /// <summary>
        /// This event is fired when the dialog is dismissed, either
        /// through user inaction, or by some other foreground action
        /// taking precedence.
        /// </summary>
        /// <remarks>
        /// For other actions (such as button presses, text field submits, etc)
        /// provide your own delegates through the fluent methods, such as
        /// <see cref="SetPositive(string label, Action<DialogButtonEventArgs> action)"/>
        /// Any async methods awaiting on the result of <see cref="ShowAsync()"/> will
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
        /// Shows the dialog. Call this method after the content and actions
        /// have been specified with the fluent methods <see cref="SetHeader(string)"/>,
        /// <see cref="SetBody(string)"/>, <see cref="SetPositive(string, Action{DialogButtonEventArgs})"/>, etc.
        /// </summary>
        /// <remarks>
        /// When implementing custom dialog types, be sure to override this to
        /// show your custom dialog in an appropriate way.
        /// </remarks>
        /// <returns> Itself, for chaining. </returns>
        IDialog Show();

        /// <summary>
        /// Returns a Task<DialogDismissedEventArgs> representing the result
        /// of the user's choice. The task will complete when the dialog is
        /// dismissed, either by user action or by some other foreground action.
        /// If the user didn't select an option, the DialogDismissedEventArgs.Choice
        /// will be null.
        /// </summary>
        /// <remarks>
        /// This is usually implemented by a TaskCompletionSource listening on the
        /// the <see cref="OnDismissed"/> delegate. This allows for either synchronous
        /// or async usage of the <see cref="IDialog"/>.
        /// </remarks>
        Task<DialogDismissedEventArgs> ShowAsync();

        /// <summary>
        /// Dismisses the dialog. Unsubscribes all listeners from the dialog's
        /// events, plays the dismiss animation, and then invokes onDismissed.
        /// OnDismissed may not be invoked immediately.
        /// </summary>
        /// <remarks>
        /// Those writing subclassed Dialogs should unsubscribe listeners from their custom
        /// events, if any, as well. Also, this base implementation should be called
        /// after your derived implementation, as it will trigger the dismissal animation
        /// and invoke the onDismissed event for you.
        /// Any async methods awaiting on the result of ShowAsync() will be completed at the
        /// same time as the OnDismissed delegate is invoked.
        /// </remarks>
        void Dismiss();

        /// <summary>
        /// The dialog's root GameObject, used for setting visibility.
        /// </summary>
        GameObject VisibleRoot { get; }
    }
}