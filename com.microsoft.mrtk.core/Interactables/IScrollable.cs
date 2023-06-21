// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A Unity <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.3/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>
    /// that is capable of being scrolled by a Unity <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.3/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractor.html">IXRInteractor</see>.
    /// </summary>
    public interface IScrollable : IXRInteractable
    {
        /// <summary>
        /// Get the transform that is backing this scrollable about.
        /// </summary>
        Transform ScrollableTransform { get; }

        /// <summary>
        /// Get if the scrollable is currently scrolling
        /// </summary>
        bool IsScrolling { get; }

        /// <summary>
        /// Get the interactor that is scrolling the transform
        /// </summary>
        IXRInteractor ScrolllingInteractor { get; }

        /// <summary>
        /// Get the anchor position at the start of the scroll
        /// </summary>
        Vector3 ScrollingAnchorPosition { get; }
    }
}
