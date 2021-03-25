// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Dwell
{
    /// <summary>
    /// Example script to demonstrate dwell visuals in sample scene
    /// </summary>
    public abstract class BaseDwellSample : MonoBehaviour
    {
        [SerializeField]
        protected Image dwellVisualImage = null;

        [SerializeField]
        protected Button targetButton = null;

        /// <summary>
        /// The dwell handler associated with the target
        /// </summary>
        protected DwellHandler DwellHandler { get; set; }

        /// <summary>
        /// Whether the targeting is being dwelled on
        /// </summary>
        protected bool IsDwelling { get; set; } = false;

        /// <summary>
        /// Assign the DwellHandler at Awake()
        /// </summary>
        protected virtual void Awake()
        {
            DwellHandler = this.GetComponent<DwellHandler>();
        }

        /// <summary>
        /// Function called when entering dwell started state
        /// </summary>
        public virtual void DwellStarted(IMixedRealityPointer pointer) { IsDwelling = true; }

        /// <summary>
        /// Function called when entering dwell intended state
        /// </summary>
        public virtual void DwellIntended(IMixedRealityPointer pointer) { }

        /// <summary>
        /// Function called when entering dwell canceled state
        /// </summary>
        public virtual void DwellCanceled(IMixedRealityPointer pointer) { IsDwelling = false; }

        /// <summary>
        /// Function called when entering dwell completed state
        /// </summary>
        public virtual void DwellCompleted(IMixedRealityPointer pointer)
        {
            IsDwelling = false;
            if (targetButton != null)
            {
                targetButton.onClick?.Invoke();
            }
        }

        /// <summary>
        /// Function called when the target button is pressed
        /// </summary>
        public virtual void ButtonExecute() { }
    }
}
