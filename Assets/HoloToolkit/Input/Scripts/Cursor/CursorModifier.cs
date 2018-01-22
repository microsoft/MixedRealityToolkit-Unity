// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Component that can be added to any game object with a collider to modify 
    /// how a cursor reacts when on that collider.
    /// </summary>
    public class CursorModifier : MonoBehaviour, ICursorModifier
    {
        [SerializeField]
        [Tooltip("Transform for which this cursor modifier applies its various properties.")]
        private Transform hostTransform;

        /// <summary>
        /// Transform for which this cursor modifies applies its various properties.
        /// </summary>
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

        [Tooltip("How much a cursor should be offset from the surface of the object when overlapping.")]
        public Vector3 CursorOffset = Vector3.zero;

        [Tooltip("Direction of the cursor offset.")]
        public Vector3 CursorNormal = Vector3.back;

        [Tooltip("Scale of the cursor when looking at this object.")]
        public Vector3 CursorScaleOffset = Vector3.one;

        [Tooltip("Should the cursor snap to the object.")]
        public bool SnapCursor = false;

        [Tooltip("If true, the normal from the pointing vector will be used to orient the cursor " +
                 "instead of the targeted object's normal at point of contact.")]
        public bool UseGazeBasedNormal = false;

        [Tooltip("Should the cursor be hiding when this object is focused.")]
        public bool HideCursorOnFocus = false;

        [SerializeField]
        [Tooltip("Cursor animation parameters to set when this object is focused. Leave empty for none.")]
        private AnimatorParameter[] cursorParameters;

        #region ICursorModifier Implementation

        public AnimatorParameter[] CursorParameters { get { return cursorParameters; } }

        /// <summary>
        /// Return whether or not hide the cursor
        /// </summary>
        /// <returns></returns>
        public bool GetCursorVisibility()
        {
            return HideCursorOnFocus;
        }

        public Vector3 GetModifiedPosition(ICursor cursor)
        {
            if (SnapCursor)
            {
                // Snap if the targeted object has a cursor modifier that supports snapping
                return HostTransform.position + HostTransform.TransformVector(CursorOffset);
            }

            FocusDetails focusDetails;
            if (FocusManager.Instance.TryGetFocusDetails(cursor.Pointer, out focusDetails))
            {
                // Else, consider the modifiers on the cursor modifier, but don't snap
                return focusDetails.Point + HostTransform.TransformVector(CursorOffset);
            }

            return Vector3.zero;
        }

        public Quaternion GetModifiedRotation(ICursor cursor)
        {
            RayStep lastStep = cursor.Pointer.Rays[cursor.Pointer.Rays.Length - 1];
            Vector3 forward = UseGazeBasedNormal ? -lastStep.Direction : HostTransform.rotation * CursorNormal;

            // Determine the cursor forward rotation
            return forward.magnitude > 0
                ? Quaternion.LookRotation(forward, Vector3.up)
                : cursor.Rotation;
        }

        public Vector3 GetModifiedScale(ICursor cursor)
        {
            return CursorScaleOffset;
        }

        public void GetModifiedTransform(ICursor cursor, out Vector3 position, out Quaternion rotation, out Vector3 scale)
        {
            position = GetModifiedPosition(cursor);
            rotation = GetModifiedRotation(cursor);
            scale = GetModifiedScale(cursor);
        }

        #endregion ICursorModifier Implementation

        #region IFocusChangedHandler Implementation

        void IFocusChangedHandler.OnBeforeFocusChange(FocusEventData eventData)
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

        void IFocusChangedHandler.OnFocusChanged(FocusEventData eventData) { }

        #endregion IFocusChangedHandler Implementation
    }
}
