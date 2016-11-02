using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Component that can be added to any collider to modify how a cursor reacts when on that collider.
    /// </summary>
    public class CursorModifier : MonoBehaviour, ICursorModifier
    {
        private void Awake()
        {
            if (HostTransform == null)
            {
                HostTransform = transform;
            }
        }

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

        private ICursor currentCursor;

        public void RegisterCursor(ICursor cursor)
        {
            currentCursor = cursor;
        }

        /// <summary>
        /// Return whether or not hide the cursor
        /// </summary>
        /// <returns></returns>
        public bool GetCursorVisibility()
        {
            return HideCursorOnFocus;
        }

        /// <summary>
        /// Get the modifier position
        /// </summary>
        /// <param name="position"></param>
        public Vector3 GetPosition()
        {
            Vector3 position;

            // Set the cursor position
            if (SnapCursor)
            {
                // Snap if the targeted object has a cursor modifier that supports snapping
                position = HostTransform.position +
                                 HostTransform.TransformVector(CursorOffset);
            }
            // Else, consider the modifiers on the cursor modifier, but don't snap
            else
            {
               position = GazeManager.Instance.HitPosition + HostTransform.TransformVector(CursorOffset);
            }

            return position;
        }

        /// <summary>
        /// Get modifier rotation
        /// </summary>
        /// <param name="rotation"></param>
        public Quaternion GetRotation()
        {
            Quaternion rotation;

            Vector3 forward = UseGazeBasedNormal ? -GazeManager.Instance.GazeNormal : HostTransform.rotation * CursorNormal;

            // Set the cursor forward
            if (forward.magnitude > 0)
            {
                rotation = Quaternion.LookRotation(forward, Vector3.up);
            }
            else
            {
                rotation = currentCursor.GetRotation();
            }

            return rotation;
        }

        /// <summary>
        /// Get modifier scale
        /// </summary>
        /// <param name="scale"></param>
        public Vector3 GetScale()
        {
            // Set cursor scale
            return CursorScaleOffset;
        }

        /// <summary>
        /// Get modifier translation comprising position, rotation and scale
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public void GetModifierTranslation(out Vector3 position, out Quaternion rotation, out Vector3 scale)
        {
            position = GetPosition();
            rotation = GetRotation();
            scale = GetScale();
        }
    }
}
