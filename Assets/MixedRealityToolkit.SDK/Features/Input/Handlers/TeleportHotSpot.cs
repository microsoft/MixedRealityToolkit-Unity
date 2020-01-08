// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Teleport
{
    /// <summary>
    /// SDK component handling teleportation to a specific position &amp; orientation when a user focuses
    /// this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> and triggers the teleport action.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/TeleportHotSpot")]
    public class TeleportHotSpot : BaseFocusHandler, IMixedRealityTeleportHotSpot
    {
        #region IMixedRealityFocusHandler Implementation

        /// <inheritdoc />
        public override void OnBeforeFocusChange(FocusEventData eventData)
        {
            base.OnBeforeFocusChange(eventData);

            if (!(eventData.Pointer is TeleportPointer)) { return; }

            IMixedRealityTeleportPointer teleportPointer = eventData.Pointer as IMixedRealityTeleportPointer;

            if (teleportPointer == null)
                return;

            if (eventData.NewFocusedObject == gameObject)
            {
                teleportPointer.TeleportHotSpot = this;

                if (teleportPointer.IsInteractionEnabled)
                {
                    CoreServices.TeleportSystem?.RaiseTeleportCanceled(eventData.Pointer, this);
                    CoreServices.TeleportSystem?.RaiseTeleportRequest(eventData.Pointer, this);
                }
            }
            else if (eventData.OldFocusedObject == gameObject)
            {
                teleportPointer.TeleportHotSpot = null;

                if (teleportPointer.IsInteractionEnabled)
                {
                    CoreServices.TeleportSystem?.RaiseTeleportCanceled(eventData.Pointer, this);
                }
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
        public bool OverrideTargetOrientation => overrideOrientation;

        /// <inheritdoc />
        public float TargetOrientation => transform.eulerAngles.y;

        /// <inheritdoc />
        public GameObject GameObjectReference => gameObject;

        #endregion IMixedRealityTeleportTarget Implementation

        private void OnDrawGizmos()
        {
            Gizmos.color = IsActive ? Color.green : Color.red;
            Gizmos.DrawLine(Position + (Vector3.up * 0.1f), Position + (Vector3.up * 0.1f) + (transform.forward * 0.1f));
            Gizmos.DrawSphere(Position + (Vector3.up * 0.1f) + (transform.forward * 0.1f), 0.01f);
        }
    }
}
