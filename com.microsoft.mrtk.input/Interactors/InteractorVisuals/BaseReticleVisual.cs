using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [AddComponentMenu("MRTK/Input/Base Reticle Visual")]
    [DisallowMultipleComponent]
    public class BaseReticleVisual : MonoBehaviour, IXRCustomReticleProvider
    {
        [SerializeField]
        private GameObject baseReticle;

        // Staging area for custom reticles that interactors can attach to show unique visuals
        protected GameObject customReticle;
        protected bool customReticleAttached;

        public GameObject reticle => customReticleAttached ? customReticle : baseReticle;

        #region IXRCustomReticleProvider Implementation

        /// <inheritdoc />
        public bool AttachCustomReticle(GameObject reticleInstance)
        {
            if (!customReticleAttached)
            {
                if (baseReticle != null)
                {
                    baseReticle.SetActive(false);
                }
            }
            else
            {
                if (customReticle != null)
                {
                    customReticle.SetActive(false);
                }
            }

            customReticle = reticleInstance;
            if (customReticle != null)
            {
                customReticle.SetActive(true);
            }

            customReticleAttached = true;

            return true;
        }

        /// <inheritdoc />
        public bool RemoveCustomReticle()
        {
            if (customReticle != null)
            {
                customReticle.SetActive(false);
            }

            // If we have a standard reticle, re-enable that one.
            if (baseReticle != null)
            {
                baseReticle.SetActive(true);
            }

            customReticle = null;
            customReticleAttached = false;
            return false;
        }

        #endregion IXRCustomReticleProvider Implementation
    }
}
