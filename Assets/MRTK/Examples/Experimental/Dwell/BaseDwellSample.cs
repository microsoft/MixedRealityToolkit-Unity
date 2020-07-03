// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
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

        protected DwellHandler dwellHandler;

        public bool isDwelling = false;

        protected virtual void Awake()
        {
            dwellHandler = this.GetComponent<DwellHandler>();
        }

        public virtual void DwellStarted(IMixedRealityPointer pointer) { isDwelling = true; }

        public virtual void DwellIntended(IMixedRealityPointer pointer) { }

        public virtual void DwellCanceled(IMixedRealityPointer pointer) { isDwelling = false; }

        public virtual void DwellCompleted(IMixedRealityPointer pointer)
        {
            isDwelling = false;
            if (targetButton != null)
            {
                targetButton.onClick?.Invoke();
            }
        }

        public virtual void ButtonExecute() { }
    }
}
