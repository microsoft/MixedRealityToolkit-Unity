// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    public abstract class BaseDwellSample : MonoBehaviour
    {
        public bool isDwelling = false;

        protected DwellHandler dwellHandler;

        [SerializeField]
        protected Image dwellVisualImage = null;

        protected virtual void Awake()
        {
            dwellHandler = this.GetComponent<DwellHandler>();
        }

        public virtual void DwellStarted(IMixedRealityPointer pointer) { isDwelling = true; }

        public virtual void DwellIntended(IMixedRealityPointer pointer) { }

        public virtual void DwellCanceled(IMixedRealityPointer pointer) { isDwelling = false; }
        
        public virtual void DwellCompleted(IMixedRealityPointer pointer) { isDwelling = false; ButtonExecute(); }

        public virtual void ButtonExecute() { }
    }
}
