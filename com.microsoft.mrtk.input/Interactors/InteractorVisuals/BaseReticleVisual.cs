// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
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
        [Tooltip("The root of the reticle visuals")]
        private Transform reticleRoot;

        /// <summary>
        /// The root of the reticle visuals. 
        /// </summary>
        /// <remarks>
        /// This transform hold both the base and custom reticle.
        /// </remarks>
        protected Transform ReticleRoot => reticleRoot;

        [SerializeField]
        [Tooltip("The reticle model to use when the interactable doesn't specify a custom one.")]
        private GameObject baseReticle;

        /// <summary>
        /// The reticle model to use when the interactable doesn't specify a custom one.
        /// </summary>
        protected GameObject BaseReticle => baseReticle;

        /// <summary>
        /// Staging area for custom reticles that interactors can attach to show unique visuals.
        /// </summary>
        protected GameObject CustomReticle
        {
            get;
            private set;
        }

        /// <summary>
        /// Is there a custom reticle currently attached to this interactor?
        /// </summary>
        protected bool CustomReticleAttached
        {
            get;
            private set;
        }

        /// <summary>
        /// The current reticle that the interactor is using.
        /// </summary>
        public GameObject Reticle => CustomReticleAttached ? CustomReticle : baseReticle;

        private IReticleVisual visual;

        /// <summary>
        /// Cached reference to the <see cref="IReticleVisual"/> component on <see cref="Reticle"/>.
        /// </summary>
        protected IReticleVisual Visual
        {
            get
            {
                if (visual == null)
                {
                    visual = Reticle.GetComponent<IReticleVisual>();
                }

                return visual;
            }
        }

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
        protected virtual void OnEnable()
        {
            // If no reticle root is specified, use the interactor's transform.
            if (reticleRoot == null)
            {
                reticleRoot = transform;
            }
        }

        #region IXRCustomReticleProvider

        /// <inheritdoc />
        public bool AttachCustomReticle(GameObject reticleInstance)
        {
            if (!CustomReticleAttached)
            {
                if (baseReticle != null)
                {
                    baseReticle.SetActive(false);
                }
            }
            else
            {
                if (CustomReticle != null)
                {
                    CustomReticle.SetActive(false);
                }
            }

            CustomReticle = reticleInstance;
            if (CustomReticle != null)
            {
                CustomReticle.SetActive(true);

                // Ensure the custom reticle is parented under this game object
                CustomReticle.transform.parent = reticleRoot;
                CustomReticle.transform.localPosition = Vector3.zero;
                CustomReticle.transform.localRotation = Quaternion.identity;
            }

            CustomReticleAttached = true;

            // Clear old references to the old Reticle components.
            visual = null;

            return true;
        }

        /// <inheritdoc />
        public bool RemoveCustomReticle()
        {
            if (CustomReticle != null)
            {
                CustomReticle.SetActive(false);
            }

            // If we have a standard reticle, re-enable that one.
            if (baseReticle != null)
            {
                baseReticle.SetActive(true);
            }

            CustomReticle = null;
            CustomReticleAttached = false;
            visual = null;

            return false;
        }

        #endregion IXRCustomReticleProvider Implementation
    }
}
