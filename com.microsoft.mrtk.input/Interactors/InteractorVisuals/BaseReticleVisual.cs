// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A base class for Reticle Visuals. This class takes care of swapping reticle models by
    /// implementing <see cref="IXRCustomReticleProvider"/>. Classes which derive from this class
    /// should provide behavior that is universal to that class of reticles, such as aligning a reticle
    /// with an interactor's pose.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Base Reticle Visual")]
    [DisallowMultipleComponent]
    public class BaseReticleVisual : MonoBehaviour, IXRCustomReticleProvider
    {
        [SerializeField]
        [Tooltip("The reticle model to use when the interactable doesn't specify a custom one.")]
        private GameObject baseReticle;

        /// <summary>
        /// Staging area for custom reticles that interactors can attach to show unique visuals.
        /// </summary>
        protected GameObject customReticle;

        /// <summary>
        /// Is there a custom reticle currently attached to this interactor?
        /// </summary>
        protected bool customReticleAttached;

        /// <summary>
        /// The current reticle that the interactor is using.
        /// </summary>
        public GameObject Reticle => customReticleAttached ? customReticle : baseReticle;

        private IVariableReticle variableReticle;

        /// <summary>
        /// Cached variable reticle reference.
        /// </summary>
        protected IVariableReticle VariableReticle
        {
            get
            {
                if (variableReticle == null)
                {
                    variableReticle = Reticle.GetComponent<IVariableReticle>();
                }

                return variableReticle;
            }
        }
        #region IXRCustomReticleProvider

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

                // Ensure the custom reticle is parented under this gameobject
                customReticle.transform.parent = transform;
                customReticle.transform.localPosition = Vector3.zero;
                customReticle.transform.localRotation = Quaternion.identity;
            }

            customReticleAttached = true;

            // Make sure that the variable reticle now refers to the correct reticle
            variableReticle = Reticle.GetComponent<IVariableReticle>();

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
            variableReticle = null;
            return false;
        }

        #endregion IXRCustomReticleProvider Implementation
    }
}
