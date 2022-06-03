// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// An interface that all interactors which
    /// must be notified of the disabling of an interactable's
    /// colliders should implement.
    /// </summary>
    /// <remarks>
    /// Colliders, when disabled, destroyed, or otherwise removed,
    /// will not fire OnTriggerExit events. This is a method of
    /// notifying interactors of these events manually.
    ///
    /// This is a semi-temporary bugfix/workaround for the lack of
    /// proper support for disabling colliders in TriggerContactMonitor
    /// in <see cref="XRDirectInteractor"/>. This interface is marked
    /// internal as it may be removed in the future if/when Unity fixes
    /// the bug. If the bug will not be fixed, the interface may be
    // made public.
    /// </remarks>
    internal interface IColliderDisabledReceiver
    {
        /// <summary>
        /// Called to manually notify that a collider has been disabled.
        /// Interactors inheriting from <see cref="XRDirectInteractor">
        /// should implement this, and call OnTriggerExit with the provided
        /// collider.
        /// </summary>
        void NotifyColliderDisabled(Collider collider);
    }
}