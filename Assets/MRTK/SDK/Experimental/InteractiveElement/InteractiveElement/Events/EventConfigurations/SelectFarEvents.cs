// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The event configuration for the SelectFar InteractionState.
    /// </summary>
    public class SelectFarEvents : BaseInteractionEventConfiguration
    {
        [SerializeField]
        [Tooltip("Whether or not to register the IMixedRealityPointerHandler for global input. If Global is true, then" +
        " events in the SelectFar state will be fired without requiring an object to be in focus. ")]
        private bool global = false;

        /// <summary>
        /// Whether or not to register the IMixedRealityPointerHandler for global input. If Global is true, then
        /// events in the SelectFar state will be fired without requiring an object to be in focus. 
        /// </summary>
        public bool Global
        {
            get => global;
            set 
            {
                global = value;
                OnGlobalChanged.Invoke();
            }
        }

        /// <summary>
        /// A Unity event used to track whether or not the Global property has changed.
        /// </summary>
        [HideInInspector]
        public UnityEvent OnGlobalChanged = new UnityEvent();

        /// <summary>
        /// A Unity event with MixedRealityPointerEventData. 
        /// </summary>
        public SelectFarInteractionEvent OnSelectDown = new SelectFarInteractionEvent();

        /// <summary>
        /// A Unity event with MixedRealityPointerEventData. 
        /// </summary>
        public SelectFarInteractionEvent OnSelectUp = new SelectFarInteractionEvent();

        /// <summary>
        /// A Unity event with MixedRealityPointerEventData. 
        /// </summary>
        public SelectFarInteractionEvent OnSelectHold = new SelectFarInteractionEvent();

        /// <summary>
        /// A Unity event with MixedRealityPointerEventData. 
        /// </summary>
        public SelectFarInteractionEvent OnSelectClicked = new SelectFarInteractionEvent();

    }
}
