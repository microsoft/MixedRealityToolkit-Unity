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
    /// This script fits a BoxCollider onto a canvas element.
    /// **You should have this script disabled most of the time for performance reasons.**
    /// By default, for performance, it only recomputes the mask intersections and collider bounds when parent
    /// dimensions have been updated, the mask has changed, or a parent scrollrect has scrolled.
    /// If you need more frequent collider updates, you can enable the script.
    /// Works in both Edit and Play modes. Only fits the X and Y axes, and preserves the Z axis.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("MRTK/UX/Rect Transform Collider Fitter")]
    public class RectTransformColliderFitter : UIBehaviour, IClippable
    {
        [SerializeField]
        [Tooltip("The collider to fit to the RectTransform.")]
        private BoxCollider thisCollider;

        /// <summary>
        /// The collider to fit to the RectTransform.
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
        [Tooltip("The RectTransform to fit the collider onto.")]
        [FormerlySerializedAs("rectTransform")]
        private RectTransform attachedRectTransform;

        public RectTransform rectTransform => attachedRectTransform;

        [SerializeField]
        [Tooltip("2D padding around the RectTransform.")]
        private Vector2 padding = Vector2.zero;

        /// <summary>
        /// 2D padding around the rect transform.
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

        private void Update()
        {
            if (rectTransform.hasChanged)
            {
                Fit();
            }
        }

        private static readonly ProfilerMarker ColliderFitterFitPerMarker =
            new ProfilerMarker("[MRTK] RectTransformColliderFitter.Fit");

        // Fits collider to RectTransform, clipping to the mask if one exists.
        private void Fit()
        {
            using (ColliderFitterFitPerMarker.Auto())
            {
                if (rectTransform == null || thisCollider == null) { return; }

                Rect computedRect = attachedRectTransform.rect;

                if (mask != null)
                {
                    // Transform the mask's rect to our local space.
                    Matrix4x4 matrix = rectTransform.worldToLocalMatrix * mask.transform.localToWorldMatrix;
                    Vector3 localClipCorner = matrix.MultiplyPoint(new Vector2(mask.rectTransform.rect.x, mask.rectTransform.rect.y));
                    Rect localClipRect = new Rect(localClipCorner.x, localClipCorner.y, mask.rectTransform.rect.width, mask.rectTransform.rect.height);

                    // Compute the intersection.
                    bool intersects = rectTransform.rect.Intersects(localClipRect, out computedRect);

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
