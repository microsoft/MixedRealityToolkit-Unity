// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Example script of how to navigate a <see cref="Microsoft.MixedReality.Toolkit.UI.ScrollingObjectCollection"/> by pagination.
    /// Allows the call to scroll pagination methods from the inspector.
    /// </summary>
    public class ScrollablePagination : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The ScrollingObjectCollection to navigate")]
        private ScrollingObjectCollection scrollView;

        /// <summary>
        /// The ScrollingObjectCollection to navigate.
        /// </summary>
        public ScrollingObjectCollection ScrollView
        {
            get
            {
                if (scrollView == null)
                {
                    scrollView = GetComponent<ScrollingObjectCollection>();
                }
                return scrollView;
            }
            set
            {
                scrollView = value;
            }
        }

        /// <summary>
        /// Smoothly moves the scroll container a relative number of tiers of cells.
        /// </summary>
        public void ScrollByTier(int amount)
        {
            Debug.Assert(ScrollView != null, "Scroll view needs to be defined before using pagination.");
            scrollView.MoveByTiers(amount);
        }
    }
}