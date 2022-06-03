// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// An interactable for the handles of a <see cref="BoundsControl"/>.
    /// Scale handles subclass this to implement custom occlusion + reorientation logic.
    /// </summary>
    internal class BoundsHandleInteractable : StatefulInteractable, ISnapInteractable
    {
        private BoundsControl boundsControlRoot;

        /// <summary>
        /// Reference to the BoundsControl that is associated with this handle.
        /// </summary>
        public BoundsControl BoundsControlRoot
        {
            get
            {
                if (boundsControlRoot == null)
                {
                    boundsControlRoot = transform.GetComponentInParent<BoundsControl>();
                }
                return boundsControlRoot;
            }
            set
            {
                boundsControlRoot = value;
            }
        }

        #region ISnapInteractable

        /// <inheritdoc />
        public Transform HandleTransform => transform;

        #endregion

        /// <summary>
        /// Is this handle currently occluded or hidden? Some handles
        /// are designed to occlude themselves in certain bounding box orientations and perspectives.
        /// </summary>
        /// <remarks>
        /// The "setter" for this is effectively processed in Update(), so that multiple per-frame
        /// calls to IsOccluded = true/false will not incur unnecessary expense.
        /// </remarks>
        public virtual bool IsOccluded { get; set; }

        public Vector3 FlattenVector { get; set; }

        public bool IsFlattened { get; set; }

        [SerializeField]
        [Tooltip("The type of handle. Affects what the BoundsControl does when this handle is grabbed.")]
        private HandleType handleType;

        /// <summary>
        /// This handle's handle type.
        /// </summary>
        public HandleType HandleType { get => handleType; set => handleType = value; }

        private MeshRenderer handleRenderer;

        private bool wasOccludedLastFrame = false;

        protected override void Awake()
        {
            base.Awake();

            // Handles are never selected by poking.
            DisableInteractorType(typeof(IPokeInteractor));

            handleRenderer = GetComponentInChildren<MeshRenderer>();

            // Start occluded, so we don't show a frame of handles
            // on startup when they should start disabled.
            // We'll un-occlude on the next frame if we need to.
            if (handleRenderer != null)
            {
                handleRenderer.enabled = false;
            }
            colliders[0].enabled = false;
            wasOccludedLastFrame = true;
        }

        protected virtual void LateUpdate()
        {
            // Do our IsOccluded "setter" in Update so we don't do this multiple times a frame.
            if (IsOccluded != wasOccludedLastFrame)
            {
                wasOccludedLastFrame = IsOccluded;
                if (handleRenderer != null)
                {
                    handleRenderer.enabled = !IsOccluded;
                }
                colliders[0].enabled = !IsOccluded;
            }

            // Maintain global scale of the handle(s).
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(1.0f / transform.lossyScale.x, 1.0f / transform.lossyScale.y, 1.0f / transform.lossyScale.z);
        }

        /// <inheritdoc />
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            BoundsControlRoot.OnHandleSelectEntered(this, args);
        }

        /// <inheritdoc />
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            BoundsControlRoot.OnHandleSelectExited(this, args);
        }
    }
}