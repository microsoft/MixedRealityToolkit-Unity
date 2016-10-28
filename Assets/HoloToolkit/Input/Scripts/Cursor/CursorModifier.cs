using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Component that can be added to any collider to modify how a cursor reacts when on that collider.
    /// </summary>
    public class CursorModifier : MonoBehaviour
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
    }
}
