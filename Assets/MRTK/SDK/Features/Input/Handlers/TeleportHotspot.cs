// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Teleport
{
    /// <summary>
    /// SDK component handling teleportation to a specific position &amp; orientation when a user focuses
    /// this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> and triggers the teleport action.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/TeleportHotSpot")]
    public class TeleportHotspot : BaseFocusHandler, IMixedRealityTeleportHotspot
    {
        #region IMixedRealityFocusHandler Implementation

        /// <inheritdoc />
        public override void OnBeforeFocusChange(FocusEventData eventData)
        {
            base.OnBeforeFocusChange(eventData);
            if (!(eventData.Pointer is IMixedRealityTeleportPointer teleportPointer) || teleportPointer.IsNull())
            {
                return;
            }

            if (eventData.NewFocusedObject == gameObject)
            {
                teleportPointer.TeleportHotspot = this;
            }
            else if (eventData.OldFocusedObject == gameObject)
            {
                teleportPointer.TeleportHotspot = null;
            }
        }

        #endregion IMixedRealityFocusHandler Implementation

        #region IMixedRealityTeleportTarget Implementation

        /// <inheritdoc />
        public Vector3 Position => transform.position;

        /// <inheritdoc />
        public Vector3 Normal => transform.up;

        /// <inheritdoc />
        public bool IsActive => isActiveAndEnabled;

        [SerializeField]
        [Tooltip("Should the destination orientation be overridden? " +
                 "Useful when you want to orient the user in a specific direction when they teleport to this position. " +
                 "Override orientation is the transform forward of the GameObject this component is attached to.")]
        private bool overrideOrientation = false;

        /// <inheritdoc />
        public bool OverrideOrientation => overrideOrientation;

        /// <inheritdoc />
        public float TargetRotation => transform.eulerAngles.y;

        /// <inheritdoc />
        public GameObject GameObjectReference => gameObject;

        #endregion IMixedRealityTeleportTarget Implementation

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = IsActive ? Color.green : Color.red;
            Handles.color = IsActive ? Color.green : Color.red;

            Gizmos.DrawLine(Position, Position + (Vector3.up * 0.5f));
            Gizmos.DrawLine(Position + (Vector3.up * 0.5f), Position + (Vector3.up * 0.5f) + (transform.forward * 0.5f));

            Handles.DrawWireDisc(Position, Vector3.up, 0.4f);
            Handles.DrawWireDisc(Position + (Vector3.up * 0.5f), Vector3.up, 0.4f);
        }
#endif
    }
}
