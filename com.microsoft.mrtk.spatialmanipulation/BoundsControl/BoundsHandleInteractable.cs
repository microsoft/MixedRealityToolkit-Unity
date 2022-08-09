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
    [AddComponentMenu("MRTK/Spatial Manipulation/Bounds Handle Interactable")]
    public class BoundsHandleInteractable : StatefulInteractable, ISnapInteractable
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

        [SerializeField]
        [Tooltip("Should the handle maintain its global size, even as the object changes size?")]
        private bool maintainGlobalSize = true;

        /// <summary>
        /// Should the handle maintain its global size, even as the object changes size?
        /// </summary>
        public bool MaintainGlobalSize { get => maintainGlobalSize; set => maintainGlobalSize = value;}

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

        /// <summary>
        /// The vector/direction along which the bounds should be flattened.
        /// Set by the box visuals script; it controls which handles are hidden
        /// when the bounds are flattened to a 2D/slate shape. Has no effect
        /// if/when IsFlattened is false!
        /// </summary>
        public Vector3 FlattenVector { get; set; }

        /// <summary>
        /// Whether the parent bounds is flattened or not. If true,
        /// FlattenVector is used to determine which axis to flatten along
        /// (and, accordingly, which handles to hide!)
        /// </summary>
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

        private Vector3 initialLocalScale;

        private float initialParentScale;

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

        // Record initial values at Start(), so that we
        // capture the bounds sizing, etc.
        void Start()
        {
            initialLocalScale = transform.localScale;
            initialParentScale = MaxComponent(transform.parent.lossyScale);
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

            // Maintain the aspect ratio/proportion of the handles, globally.
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(1.0f / transform.lossyScale.x,
                                               1.0f / transform.lossyScale.y,
                                               1.0f / transform.lossyScale.z);
            
            // If we don't want to maintain the overall *size*, we scale
            // by the maximum component of the box so that the handles grow/shrink
            // with the overall box manipulation.
            if (!maintainGlobalSize)
            {
                transform.localScale = transform.localScale * (MaxComponent(transform.parent.lossyScale) / initialParentScale);
            }
        }

        private float MaxComponent(Vector3 v)
        {
            return Mathf.Max(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
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