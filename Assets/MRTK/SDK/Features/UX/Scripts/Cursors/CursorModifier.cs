// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Component that can be added to any <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>
    /// with a <see href="https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see> to
    /// modify the <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityCursor"/> reacts when
    /// focused by a <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointer"/>.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/CursorModifier")]
    public class CursorModifier : MonoBehaviour, ICursorModifier
    {
        #region ICursorModifier Implementation

        [SerializeField]
        [Tooltip("Transform for which this cursor modifier applies its various properties.")]
        private Transform hostTransform;

        /// <inheritdoc />
        public Transform HostTransform
        {
            get
            {
                if (hostTransform == null)
                {
                    hostTransform = transform;
                }

                return hostTransform;
            }
            set { hostTransform = value; }
        }

        [SerializeField]
        [Tooltip("How much a cursor should be offset from the surface of the object when overlapping.")]
        private Vector3 cursorPositionOffset = Vector3.zero;

        /// <inheritdoc />
        public Vector3 CursorPositionOffset
        {
            get
            {
                return cursorPositionOffset;
            }
            set
            {
                cursorPositionOffset = value;
            }
        }

        [SerializeField]
        [Tooltip("Should the cursor snap to the GameObject?")]
        private bool snapCursorPosition = false;

        /// <inheritdoc />
        public bool SnapCursorPosition
        {
            get
            {
                return snapCursorPosition;
            }
            set
            {
                snapCursorPosition = value;
            }
        }

        [Tooltip("Scale of the cursor when looking at this GameObject.")]
        [SerializeField]
        private Vector3 cursorScaleOffset = Vector3.one;

        /// <inheritdoc />
        public Vector3 CursorScaleOffset
        {
            get
            {
                return cursorScaleOffset;
            }
            set
            {
                cursorScaleOffset = value;
            }
        }

        [SerializeField]
        [Tooltip("Direction of the cursor offset.")]
        private Vector3 cursorNormalOffset = Vector3.back;

        /// <inheritdoc />
        public Vector3 CursorNormalOffset
        {
            get
            {
                return cursorNormalOffset;
            }
            set
            {
                cursorNormalOffset = value;
            }
        }

        [SerializeField]
        [Tooltip("If true, the normal from the pointing vector will be used to orient the cursor instead of the targeted object's normal at point of contact.")]
        private bool useGazeBasedNormal = false;

        /// <inheritdoc />
        public bool UseGazeBasedNormal
        {
            get
            {
                return useGazeBasedNormal;
            }
            set
            {
                useGazeBasedNormal = value;
            }
        }

        [SerializeField]
        [Tooltip("Should the cursor be hiding when this object is focused?")]
        private bool hideCursorOnFocus = false;

        /// <inheritdoc />
        public bool HideCursorOnFocus
        {
            get
            {
                return hideCursorOnFocus;
            }
            set
            {
                hideCursorOnFocus = value;
            }
        }

        [SerializeField]
        [Tooltip("Cursor animation parameters to set when this object is focused. Leave empty for none.")]
        private AnimatorParameter[] cursorParameters = null;

        /// <inheritdoc />
        public AnimatorParameter[] CursorParameters => cursorParameters;

        /// <inheritdoc />
        public bool GetCursorVisibility() => HideCursorOnFocus;

        public Vector3 GetModifiedPosition(IMixedRealityCursor cursor)
        {
            if (SnapCursorPosition)
            {
                // Snap if the targeted object has a cursor modifier that supports snapping
                return HostTransform.position + HostTransform.TransformVector(CursorPositionOffset);
            }

            if (cursor.Pointer == null)
            {
                Debug.LogError($"{cursor.GameObjectReference.name} has no pointer set in its cursor component!");
                return Vector3.zero;
            }

            FocusDetails focusDetails;
            if (CoreServices.InputSystem?.FocusProvider != null &&
                CoreServices.InputSystem.FocusProvider.TryGetFocusDetails(cursor.Pointer, out focusDetails))
            {
                // Else, consider the modifiers on the cursor modifier, but don't snap
                return focusDetails.Point + HostTransform.TransformVector(CursorPositionOffset);
            }

            return Vector3.zero;
        }

        /// <inheritdoc />
        public Quaternion GetModifiedRotation(IMixedRealityCursor cursor)
        {
            RayStep lastStep = cursor.Pointer.Rays[cursor.Pointer.Rays.Length - 1];
            Vector3 forward = UseGazeBasedNormal ? -lastStep.Direction : HostTransform.rotation * CursorNormalOffset;

            // Determine the cursor forward rotation
            return forward.magnitude > 0
                    ? Quaternion.LookRotation(forward, Vector3.up)
                    : cursor.Rotation;
        }

        /// <inheritdoc />
        public Vector3 GetModifiedScale(IMixedRealityCursor cursor)
        {
            return CursorScaleOffset;
        }

        /// <inheritdoc />
        public void GetModifiedTransform(IMixedRealityCursor cursor, out Vector3 position, out Quaternion rotation, out Vector3 scale)
        {
            position = GetModifiedPosition(cursor);
            rotation = GetModifiedRotation(cursor);
            scale = GetModifiedScale(cursor);
        }

        #endregion ICursorModifier Implementation

        #region IMixedRealityFocusChangedHandler Implementation

        /// <inheritdoc />
        void IMixedRealityFocusChangedHandler.OnBeforeFocusChange(FocusEventData eventData)
        {
            if (eventData.NewFocusedObject == gameObject)
            {
                eventData.Pointer.CursorModifier = this;
            }

            if (eventData.OldFocusedObject == gameObject)
            {
                eventData.Pointer.CursorModifier = null;
            }
        }

        /// <inheritdoc />
        void IMixedRealityFocusChangedHandler.OnFocusChanged(FocusEventData eventData) { }

        #endregion IMixedRealityFocusChangedHandler Implementation

        #region MonoBehaviour Implementation

        private void OnValidate()
        {   // This is an appropriate use of OnValidate
            Debug.Assert(HostTransform.GetComponent<Collider>() != null, $"A collider component is required on {hostTransform.gameObject.name} for the cursor modifier component on {gameObject.name} to function properly.");
        }

        #endregion MonoBehaviour Implementation
    }
}
