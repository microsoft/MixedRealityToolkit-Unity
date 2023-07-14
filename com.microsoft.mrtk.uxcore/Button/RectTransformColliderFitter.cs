// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.Profiling;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// This script fits a Unity <see href="https://docs.unity3d.com/ScriptReference/BoxCollider.html">BoxCollider</see>
    /// component onto a canvas element.
    /// </summary>
    /// <remarks>
    /// This script should be disabled most of the time for performance reasons.
    /// By default, for performance, this class will only recomputes the mask intersections and collider bounds when parent
    /// dimensions have been updated, the mask has changed, or a parent scroll rectangle has scrolled.
    /// If you need more frequent collider updates, you can enable the script.
    ///
    /// This script works in both edit and play modes.
    ///
    /// Only fits the X and Y axes values will be modified, while the z axis value is preserved.
    /// </remarks>
    [ExecuteAlways]
    [AddComponentMenu("MRTK/UX/Rect Transform Collider Fitter")]
    public class RectTransformColliderFitter : UIBehaviour, IClippable
    {
        [SerializeField]
        [Tooltip("The collider to fit to the AttachedRectTransform.")]
        private BoxCollider thisCollider;

        /// <summary>
        /// The collider to fit to the AttachedRectTransform.
        /// </summary>
        public BoxCollider ThisCollider
        {
            get
            {
                if (thisCollider == null)
                {
                    thisCollider = GetComponent<BoxCollider>();
                }
                return thisCollider;
            }
            set => thisCollider = value;
        }

        [SerializeField]
        [Tooltip("The AttachedRectTransform to fit the collider onto.")]
        [FormerlySerializedAs("rectTransform")]
        private RectTransform attachedRectTransform;

        /// <summary>
        /// Get the attached Unity rect transform.
        /// </summary>
        public RectTransform AttachedRectTransform => attachedRectTransform;

        /// <summary>
        /// Get the attached Unity rect transform.
        /// </summary>
        RectTransform IClippable.rectTransform => attachedRectTransform;

        [SerializeField]
        [Tooltip("2D padding around the AttachedRectTransform.")]
        private Vector2 padding = Vector2.zero;

        /// <summary>
        /// Get or set the 2D padding around the rect transform.
        /// </summary>
        public Vector2 Padding
        {
            get => padding;
            set => padding = value;
        }

        [SerializeField]
        [Tooltip("When true, this will force the collider to update every frame. " +
                 "Usually unnecessary, and bad for performance. " +
                 "When false, this script will disable itself on startup." +
                 "To control this from code, simply use .enabled on the component.")]
        private bool forceUpdateEveryFrame;

        private Canvas canvas;

        private RectMask2D mask;

        private ScrollRect scrollRect;

        #region IClippable

        /// <inheritdoc />
        public void Cull(Rect clipRect, bool validRect) { }

        /// <inheritdoc />
        public virtual void RecalculateClipping() => Fit();

        /// <inheritdoc />
        public virtual void SetClipRect(Rect clipRect, bool validRect) => Fit();

        /// <inheritdoc />
        public virtual void SetClipSoftness(Vector2 clipSoftness) { }

        #endregion

        /// <summary>
        /// A Unity event function that is called when an enabled script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
            }

            if (attachedRectTransform == null)
            {
                attachedRectTransform = GetComponent<RectTransform>();
            }

            if (mask == null)
            {
                mask = GetComponentInParent<RectMask2D>();
            }

            if (mask != null)
            {
                mask.AddClippable(this);
            }

            if (scrollRect == null)
            {
                scrollRect = GetComponentInParent<ScrollRect>();
            }

            if (scrollRect != null)
            {
                scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
            }

            // Fit once on awake.
            Fit();

            // Make sure we disable on awake if we're not forced to update every frame.
            enabled = forceUpdateEveryFrame;
        }

        /// <summary>
        /// A Unity event function that is called when the script component has been destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            if (scrollRect != null)
            {
                scrollRect.onValueChanged.RemoveListener(OnScrollRectValueChanged);
            }

            base.OnDestroy();
        }

        // Compute masking and apply rect when scrolled.
        private void OnScrollRectValueChanged(Vector2 pos) => Fit();

        /// <inheritdoc />
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            Fit();
        }

        /// <inheritdoc />
        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            Awake();
        }

        /// <summary>
        /// A Unity event function that is called every frame, if this object is enabled.
        /// </summary>
        protected virtual void Update()
        {
            if (AttachedRectTransform.hasChanged)
            {
                Fit();
            }
        }

        private static readonly ProfilerMarker ColliderFitterFitPerMarker =
            new ProfilerMarker("[MRTK] RectTransformColliderFitter.Fit");

        // Fits collider to AttachedRectTransform, clipping to the mask if one exists.
        private void Fit()
        {
            using (ColliderFitterFitPerMarker.Auto())
            {
                if (AttachedRectTransform == null || thisCollider == null) { return; }

                Rect computedRect = attachedRectTransform.rect;

                if (mask != null)
                {
                    // Transform the mask's rect to our local space.
                    Matrix4x4 matrix = AttachedRectTransform.worldToLocalMatrix * mask.transform.localToWorldMatrix;
                    Vector3 localClipCorner = matrix.MultiplyPoint(new Vector2(mask.rectTransform.rect.x, mask.rectTransform.rect.y));
                    Rect localClipRect = new Rect(localClipCorner.x, localClipCorner.y, mask.rectTransform.rect.width, mask.rectTransform.rect.height);

                    // Compute the intersection.
                    bool intersects = AttachedRectTransform.rect.Intersects(localClipRect, out computedRect);

                    if (thisCollider != null)
                    {
                        // If no intersection at all, kill the collider.
                        if (!intersects)
                        {
                            thisCollider.enabled = false;
                            return;
                        }
                        else if (!thisCollider.enabled)
                        {
                            thisCollider.enabled = true;
                        }
                    }
                }

                // Apply the rect to the collider.
                if (thisCollider != null)
                {
                    thisCollider.size = new Vector3(Mathf.Abs(computedRect.width) + 2 * padding.x, Mathf.Abs(computedRect.height) + 2 * padding.y, thisCollider.size.z);
                    thisCollider.center = new Vector3(computedRect.center.x, computedRect.center.y, thisCollider.center.z);
                }
            }
        }
    }
}
