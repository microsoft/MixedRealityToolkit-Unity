// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The cursor will display the context specified in this component if it is part of the targeted object
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/CursorContextInfo")]
    public class CursorContextInfo : MonoBehaviour
    {
        public enum CursorAction
        {
            None = 0,
            Move,
            Rotate,
            Scale
        }

        [SerializeField]
        [Tooltip("Determines the context state when this object is targeted.")]
        private CursorAction currentCursorAction = CursorAction.None;

        /// <summary>
        /// Determines the context state when this object is targeted.
        /// </summary>
        public CursorAction CurrentCursorAction
        {
            get => currentCursorAction;
            set { currentCursorAction = value; }
        }

        [SerializeField]
        [Tooltip("Used to calculate the orientation of context cursors.")]
        private Transform objectCenter = null;

        /// <summary>
        /// Used to calculate the orientation of context cursors.
        /// </summary>
        public Transform ObjectCenter
        {
            get => objectCenter;
            set { objectCenter = value; }
        }
    }
}