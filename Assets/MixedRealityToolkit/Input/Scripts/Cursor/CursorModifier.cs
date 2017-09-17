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
        [Tooltip("Transform for which this cursor modifier applies its various properties.")]
        public Transform HostTransform;

        [Tooltip("How much a cursor should be offset from the surface of the object when overlapping.")]
        public Vector3 CursorOffset = Vector3.zero;

        [Tooltip("Direction of the cursor offset.")]
        public Vector3 CursorNormal = Vector3.back;

        [Tooltip("Scale of the cursor when looking at this object.")]
        public Vector3 CursorScaleOffset = Vector3.one;

        [Tooltip("Should the cursor snap to the object.")]
        public bool SnapCursor = false;

        [Tooltip("If true, the normal from the gaze vector will be used to orient the cursor " +
                 "instead of the targeted object's normal at point of contact.")]
        public bool UseGazeBasedNormal = false;

        [Tooltip("Should the cursor be hidding when this object is focused.")]
        public bool HideCursorOnFocus = false;

        [Tooltip("Cursor animation event to trigger when this object is gazed. Leave empty for none.")]
        public string CursorTriggerName;

        private void Awake()
        {
            if (HostTransform == null)
            {
                HostTransform = transform;
            }
        }

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
            Vector3 position;

            if (SnapCursor)
            {
                // Snap if the targeted object has a cursor modifier that supports snapping
                position = HostTransform.position +
                           HostTransform.TransformVector(CursorOffset);
            }
            else
            {
                // Else, consider the modifiers on the cursor modifier, but don't snap
                position = GazeManager.Instance.HitPosition + HostTransform.TransformVector(CursorOffset);
            }

            return position;
        }

        public Quaternion GetModifiedRotation(ICursor cursor)
        {
            Quaternion rotation;

            Vector3 forward = UseGazeBasedNormal ? -GazeManager.Instance.GazeNormal : HostTransform.rotation * CursorNormal;

            // Determine the cursor forward
            if (forward.magnitude > 0)
            {
                rotation = Quaternion.LookRotation(forward, Vector3.up);
            }
            else
            {
                rotation = cursor.Rotation;
            }

            return rotation;
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
    }
}
